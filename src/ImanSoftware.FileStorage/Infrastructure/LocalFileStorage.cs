// مسیر: Infrastructure/FileStorage/LocalFileStorage.cs
using ImanSoftware.FileStorage;
using ImanSoftware.Logging;
using ImanSoftware.Outcomes;
using Microsoft.Extensions.Options;
using System.Security;

namespace ImanSoftware.FileStorage;

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

    public async Task<Outcome<string>> UploadAsync(Stream fileStream, string fileName, string? container = null)
    {
        try
        {
            // اعتبارسنجی
            var validationResult = ValidateFile(fileName, fileStream.Length);
            if (!validationResult.IsSuccess)
                return Outcome.Failure<string>(validationResult.Error);

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
            return Outcome<string>.Successful(filePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file: {FileName}", fileName);
            return Outcome.Failure<string>(new OutcomeError(ex.Message, "FILE_UPLOAD_ERROR", OutcomeErrorType.Failure));
        }
    }

    public async Task<Outcome<string>> UploadAsync(byte[] fileBytes, string fileName, string? container = null)
    {
        using var stream = new MemoryStream(fileBytes);
        return await UploadAsync(stream, fileName, container);
    }

    #endregion

    #region IFileDownloader Implementation

    public async Task<Outcome<Stream>> DownloadAsync(string filePath)
    {
        try
        {
            var fullPath = GetFullPath(filePath);
            if (!File.Exists(fullPath))
                return Outcome.Failure<Stream>(new OutcomeError("File not found.", "FILE_NOT_FOUND", OutcomeErrorType.NotFound));

            var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            return Outcome.Successful<Stream>(stream);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading file: {FilePath}", filePath);
            return Outcome.Failure<Stream>(new OutcomeError(ex.Message, "FILE_DOWNLOAD_ERROR", OutcomeErrorType.Failure));
        }
    }

    public async Task<Outcome<byte[]>> DownloadBytesAsync(string filePath)
    {
        var result = await DownloadAsync(filePath);
        if (!result.IsSuccess)
            return Outcome.Failure<byte[]>(result.Error);

        try
        {
            using var memoryStream = new MemoryStream();
            await result.Data.CopyToAsync(memoryStream);
            return Outcome<byte[]>.Successful(memoryStream.ToArray());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading file bytes: {FilePath}", filePath);
            return Outcome.Failure<byte[]>(new OutcomeError(ex.Message, "FILE_READ_ERROR", OutcomeErrorType.Failure));
        }
        finally
        {
            result.Data?.Dispose();
        }
    }

    #endregion

    #region IFileDeleter Implementation

    public Task<Outcome> DeleteAsync(string filePath)
    {
        try
        {
            var fullPath = GetFullPath(filePath);
            if (!File.Exists(fullPath))
                return Task.FromResult(Outcome.Failure(new OutcomeError("File not found.", "FILE_NOT_FOUND", OutcomeErrorType.NotFound)));

            File.Delete(fullPath);
            _logger.LogInformation("File deleted successfully: {FilePath}", filePath);
            return Task.FromResult(Outcome.Successful());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file: {FilePath}", filePath);
            return Task.FromResult(Outcome.Failure(new OutcomeError(ex.Message, "FILE_DELETE_ERROR", OutcomeErrorType.Failure)));
        }
    }

    #endregion

    #region IFileInfoProvider Implementation

    public Task<Outcome<bool>> ExistsAsync(string filePath)
    {
        try
        {
            var fullPath = GetFullPath(filePath);
            return Task.FromResult(Outcome<bool>.Successful(File.Exists(fullPath)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking file existence: {FilePath}", filePath);
            return Task.FromResult(Outcome.Failure<bool>(new OutcomeError(ex.Message, "FILE_EXISTS_ERROR", OutcomeErrorType.Failure)));
        }
    }

    public Task<Outcome<FileMetadata>> GetMetadataAsync(string filePath)
    {
        try
        {
            var fullPath = GetFullPath(filePath);
            if (!File.Exists(fullPath))
                return Task.FromResult(Outcome.Failure<FileMetadata>(new OutcomeError("File not found.", "FILE_NOT_FOUND", OutcomeErrorType.NotFound)));

            var fileInfo = new FileInfo(fullPath);
            var metadata = new FileMetadata
            {
                FileName = Path.GetFileName(filePath),
                Size = fileInfo.Length,
                ContentType = GetContentType(Path.GetExtension(filePath)),
                CreatedAt = fileInfo.CreationTime,
                LastModifiedAt = fileInfo.LastWriteTime
            };

            return Task.FromResult(Outcome.Successful<FileMetadata>(metadata));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting file metadata: {FilePath}", filePath);
            return Task.FromResult(Outcome.Failure<FileMetadata>(new OutcomeError(ex.Message, "FILE_METADATA_ERROR", OutcomeErrorType.Failure)));
        }
    }

    public Task<Outcome<string>> GetUrlAsync(string filePath)
    {
        // در Local File Storage، معمولاً URL نسبی یا کامل برمی‌گردد
        // این بستگی به نحوه‌ی تنظیم Static Files در پروژه دارد
        var url = $"/uploads/{filePath.Replace('\\', '/')}";
        return Task.FromResult(Outcome<string>.Successful(url));
    }

    #endregion

    #region IFileMover Implementation

    public Task<Outcome> MoveAsync(string sourcePath, string destinationPath)
    {
        try
        {
            var fullSource = GetFullPath(sourcePath);
            var fullDest = GetFullPath(destinationPath);

            if (!File.Exists(fullSource))
                return Task.FromResult(Outcome.Failure(new OutcomeError("Source file not found.", "FILE_NOT_FOUND", OutcomeErrorType.NotFound)));

            // ایجاد پوشه‌ی مقصد
            var destDirectory = Path.GetDirectoryName(fullDest);
            if (!string.IsNullOrEmpty(destDirectory) && !Directory.Exists(destDirectory))
            {
                Directory.CreateDirectory(destDirectory);
            }

            File.Move(fullSource, fullDest);
            _logger.LogInformation("File moved successfully: {Source} → {Destination}", sourcePath, destinationPath);
            return Task.FromResult(Outcome.Successful());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moving file: {Source} → {Destination}", sourcePath, destinationPath);
            return Task.FromResult(Outcome.Failure(new OutcomeError(ex.Message, "FILE_MOVE_ERROR", OutcomeErrorType.Failure)));
        }
    }

    public Task<Outcome> CopyAsync(string sourcePath, string destinationPath)
    {
        try
        {
            var fullSource = GetFullPath(sourcePath);
            var fullDest = GetFullPath(destinationPath);

            if (!File.Exists(fullSource))
                return Task.FromResult(Outcome.Failure(new OutcomeError("Source file not found.", "FILE_NOT_FOUND", OutcomeErrorType.NotFound)));

            var destDirectory = Path.GetDirectoryName(fullDest);
            if (!string.IsNullOrEmpty(destDirectory) && !Directory.Exists(destDirectory))
            {
                Directory.CreateDirectory(destDirectory);
            }

            File.Copy(fullSource, fullDest, overwrite: true);
            _logger.LogInformation("File copied successfully: {Source} → {Destination}", sourcePath, destinationPath);
            return Task.FromResult(Outcome.Successful());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error copying file: {Source} → {Destination}", sourcePath, destinationPath);
            return Task.FromResult(Outcome.Failure(new OutcomeError(ex.Message, "FILE_COPY_ERROR", OutcomeErrorType.Failure)));
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

    private Outcome ValidateFile(string fileName, long fileSize)
    {
        // بررسی حجم فایل
        if (fileSize > _options.MaxFileSize)
        {
            var sizeInMB = _options.MaxFileSize / (1024 * 1024);
            return Outcome.Failure(new OutcomeError(
                $"File size exceeds maximum allowed ({sizeInMB} MB).",
                "FILE_TOO_LARGE",
                OutcomeErrorType.Validation));
        }

        // بررسی پسوند فایل
        if (_options.AllowedExtensions.Length > 0)
        {
            var extension = Path.GetExtension(fileName);
            if (!_options.AllowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
            {
                return Outcome.Failure(new OutcomeError(
                    $"File extension '{extension}' is not allowed.",
                    "FILE_EXTENSION_NOT_ALLOWED",
                    OutcomeErrorType.Validation));
            }
        }

        return Outcome.Successful();
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