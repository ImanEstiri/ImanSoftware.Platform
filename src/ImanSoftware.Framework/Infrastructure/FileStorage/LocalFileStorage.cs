// مسیر: Infrastructure/FileStorage/LocalFileStorage.cs
using ImanSoftware.Framework.Abstractions.FileStorage;
using ImanSoftware.Framework.Abstractions.Logging;
using ImanSoftware.Framework.Core.FileStorage;
using ImanSoftware.Framework.Core.Results;
using Microsoft.Extensions.Options;
using System.Security;
using System.Security.Cryptography;

namespace ImanSoftware.Framework.Infrastructure.FileStorage;

/// <summary>
/// Default implementation of IFileStorage using the local file system.
/// </summary>
public class LocalFileStorage : IFileStorage
{
    private readonly LocalFileStorageOptions _options;
    private readonly ILoggerAdapter<LocalFileStorage> _logger;

    public LocalFileStorage(IOptions<LocalFileStorageOptions> options, ILoggerAdapter<LocalFileStorage> logger)
    {
        _options = options.Value;
        _logger = logger;

        // ایجاد پوشه‌ی ریشه در صورت عدم وجود
        if (!Directory.Exists(_options.RootPath))
        {
            Directory.CreateDirectory(_options.RootPath);
        }
    }

    #region IFileUploader Implementation

    public async Task<Result<string>> UploadAsync(Stream fileStream, string fileName, string? container = null)
    {
        try
        {
            // اعتبارسنجی
            var validationResult = ValidateFile(fileName, fileStream.Length);
            if (!validationResult.IsSuccess)
                return Result.Failure<string>(validationResult.Error);

            var filePath = GenerateFilePath(fileName, container);
            var fullPath = Path.Combine(_options.RootPath, filePath);

            // ایجاد پوشه‌ی مقصد در صورت عدم وجود
            var directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // ذخیره‌سازی فایل
            using var fileStreamOutput = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
            await fileStream.CopyToAsync(fileStreamOutput);

            _logger.LogInformation("File uploaded successfully: {FilePath}", filePath);
            return Result<string>.Successful(filePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file: {FileName}", fileName);
            return Result.Failure<string>(new AppError(ex.Message, "FILE_UPLOAD_ERROR", ErrorType.Failure));
        }
    }

    public async Task<Result<string>> UploadAsync(byte[] fileBytes, string fileName, string? container = null)
    {
        using var stream = new MemoryStream(fileBytes);
        return await UploadAsync(stream, fileName, container);
    }

    #endregion

    #region IFileDownloader Implementation

    public async Task<Result<Stream>> DownloadAsync(string filePath)
    {
        try
        {
            var fullPath = GetFullPath(filePath);
            if (!File.Exists(fullPath))
                return Result.Failure<Stream>(new AppError("File not found.", "FILE_NOT_FOUND", ErrorType.NotFound));

            var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            return Result.Successful<Stream>(stream);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading file: {FilePath}", filePath);
            return Result.Failure<Stream>(new AppError(ex.Message, "FILE_DOWNLOAD_ERROR", ErrorType.Failure));
        }
    }

    public async Task<Result<byte[]>> DownloadBytesAsync(string filePath)
    {
        var result = await DownloadAsync(filePath);
        if (!result.IsSuccess)
            return Result.Failure<byte[]>(result.Error);

        try
        {
            using var memoryStream = new MemoryStream();
            await result.Data.CopyToAsync(memoryStream);
            return Result<byte[]>.Successful(memoryStream.ToArray());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading file bytes: {FilePath}", filePath);
            return Result.Failure<byte[]>(new AppError(ex.Message, "FILE_READ_ERROR", ErrorType.Failure));
        }
        finally
        {
            result.Data?.Dispose();
        }
    }

    #endregion

    #region IFileDeleter Implementation

    public Task<Result> DeleteAsync(string filePath)
    {
        try
        {
            var fullPath = GetFullPath(filePath);
            if (!File.Exists(fullPath))
                return Task.FromResult(Result.Failure(new AppError("File not found.", "FILE_NOT_FOUND", ErrorType.NotFound)));

            File.Delete(fullPath);
            _logger.LogInformation("File deleted successfully: {FilePath}", filePath);
            return Task.FromResult(Result.Successful());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file: {FilePath}", filePath);
            return Task.FromResult(Result.Failure(new AppError(ex.Message, "FILE_DELETE_ERROR", ErrorType.Failure)));
        }
    }

    #endregion

    #region IFileInfoProvider Implementation

    public Task<Result<bool>> ExistsAsync(string filePath)
    {
        try
        {
            var fullPath = GetFullPath(filePath);
            return Task.FromResult(Result<bool>.Successful(File.Exists(fullPath)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking file existence: {FilePath}", filePath);
            return Task.FromResult(Result.Failure<bool>(new AppError(ex.Message, "FILE_EXISTS_ERROR", ErrorType.Failure)));
        }
    }

    public Task<Result<FileMetadata>> GetMetadataAsync(string filePath)
    {
        try
        {
            var fullPath = GetFullPath(filePath);
            if (!File.Exists(fullPath))
                return Task.FromResult(Result.Failure<FileMetadata>(new AppError("File not found.", "FILE_NOT_FOUND", ErrorType.NotFound)));

            var fileInfo = new FileInfo(fullPath);
            var metadata = new FileMetadata
            {
                FileName = Path.GetFileName(filePath),
                Size = fileInfo.Length,
                ContentType = GetContentType(Path.GetExtension(filePath)),
                CreatedAt = fileInfo.CreationTime,
                LastModifiedAt = fileInfo.LastWriteTime
            };

            return Task.FromResult(Result.Successful<FileMetadata>(metadata));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting file metadata: {FilePath}", filePath);
            return Task.FromResult(Result.Failure<FileMetadata>(new AppError(ex.Message, "FILE_METADATA_ERROR", ErrorType.Failure)));
        }
    }

    public Task<Result<string>> GetUrlAsync(string filePath)
    {
        // در Local File Storage، معمولاً URL نسبی یا کامل برمی‌گردد
        // این بستگی به نحوه‌ی تنظیم Static Files در پروژه دارد
        var url = $"/uploads/{filePath.Replace('\\', '/')}";
        return Task.FromResult(Result<string>.Successful(url));
    }

    #endregion

    #region IFileMover Implementation

    public Task<Result> MoveAsync(string sourcePath, string destinationPath)
    {
        try
        {
            var fullSource = GetFullPath(sourcePath);
            var fullDest = GetFullPath(destinationPath);

            if (!File.Exists(fullSource))
                return Task.FromResult(Result.Failure(new AppError("Source file not found.", "FILE_NOT_FOUND", ErrorType.NotFound)));

            // ایجاد پوشه‌ی مقصد
            var destDirectory = Path.GetDirectoryName(fullDest);
            if (!string.IsNullOrEmpty(destDirectory) && !Directory.Exists(destDirectory))
            {
                Directory.CreateDirectory(destDirectory);
            }

            File.Move(fullSource, fullDest);
            _logger.LogInformation("File moved successfully: {Source} → {Destination}", sourcePath, destinationPath);
            return Task.FromResult(Result.Successful());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moving file: {Source} → {Destination}", sourcePath, destinationPath);
            return Task.FromResult(Result.Failure(new AppError(ex.Message, "FILE_MOVE_ERROR", ErrorType.Failure)));
        }
    }

    public Task<Result> CopyAsync(string sourcePath, string destinationPath)
    {
        try
        {
            var fullSource = GetFullPath(sourcePath);
            var fullDest = GetFullPath(destinationPath);

            if (!File.Exists(fullSource))
                return Task.FromResult(Result.Failure(new AppError("Source file not found.", "FILE_NOT_FOUND", ErrorType.NotFound)));

            var destDirectory = Path.GetDirectoryName(fullDest);
            if (!string.IsNullOrEmpty(destDirectory) && !Directory.Exists(destDirectory))
            {
                Directory.CreateDirectory(destDirectory);
            }

            File.Copy(fullSource, fullDest, overwrite: true);
            _logger.LogInformation("File copied successfully: {Source} → {Destination}", sourcePath, destinationPath);
            return Task.FromResult(Result.Successful());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error copying file: {Source} → {Destination}", sourcePath, destinationPath);
            return Task.FromResult(Result.Failure(new AppError(ex.Message, "FILE_COPY_ERROR", ErrorType.Failure)));
        }
    }

    #endregion

    #region Private Methods

    private string GetFullPath(string filePath)
    {
        // جلوگیری از Path Traversal attacks
        var normalizedPath = Path.GetFullPath(Path.Combine(_options.RootPath, filePath));
        if (!normalizedPath.StartsWith(_options.RootPath, StringComparison.OrdinalIgnoreCase))
            throw new SecurityException("Invalid path traversal attempt.");

        return normalizedPath;
    }

    private string GenerateFilePath(string fileName, string? container)
    {
        // ایجاد نام فایل یکتا با GUID
        var extension = Path.GetExtension(fileName);
        var uniqueName = $"{Guid.NewGuid():N}{extension}";

        // ساخت مسیر
        var path = string.Empty;
        if (_options.UseDateSubdirectories)
        {
            var now = DateTime.Now;
            path = Path.Combine(now.ToString("yyyy"), now.ToString("MM"), now.ToString("dd"));
        }

        if (!string.IsNullOrEmpty(container))
        {
            path = Path.Combine(path, container);
        }

        return Path.Combine(path, uniqueName);
    }

    private Result ValidateFile(string fileName, long fileSize)
    {
        // بررسی حجم فایل
        if (fileSize > _options.MaxFileSize)
        {
            var sizeInMB = _options.MaxFileSize / (1024 * 1024);
            return Result.Failure(new AppError(
                $"File size exceeds maximum allowed ({sizeInMB} MB).",
                "FILE_TOO_LARGE",
                ErrorType.Validation));
        }

        // بررسی پسوند فایل
        if (_options.AllowedExtensions.Length > 0)
        {
            var extension = Path.GetExtension(fileName);
            if (!_options.AllowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
            {
                return Result.Failure(new AppError(
                    $"File extension '{extension}' is not allowed.",
                    "FILE_EXTENSION_NOT_ALLOWED",
                    ErrorType.Validation));
            }
        }

        return Result.Successful();
    }

    private string GetContentType(string extension)
    {
        return extension.ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".txt" => "text/plain",
            ".json" => "application/json",
            ".xml" => "application/xml",
            _ => "application/octet-stream"
        };
    }

    #endregion
}