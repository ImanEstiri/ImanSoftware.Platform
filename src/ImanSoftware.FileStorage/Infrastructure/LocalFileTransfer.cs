using ImanSoftware.Logging;
using ImanSoftware.Outcomes;
using Microsoft.Extensions.Options;
using System.Security;

namespace ImanSoftware.FileStorage;

/// <summary>
/// Local file system implementation of IFileTransfer.
/// Requires configuration (RootPath, MaxFileSize, etc.).
/// </summary>
public class LocalFileTransfer : IFileTransfer
{
    private readonly FileTransferOptions _options;
    private readonly ILoggerAdapter<LocalFileTransfer> _logger;

    public LocalFileTransfer(IOptions<FileTransferOptions> options, ILoggerAdapter<LocalFileTransfer> logger)
    {
        _options = options.Value;
        _logger = logger;

        if (!Directory.Exists(_options.RootPath))
            Directory.CreateDirectory(_options.RootPath);
    }

    // ============================================================
    // IFileUploader
    // ============================================================
    public async Task<Outcome<string>> UploadAsync(Stream fileStream, string fileName, string? container = null)
    {
        try
        {
            var validation = ValidateFile(fileName, fileStream.Length);
            if (!validation.IsSuccess)
                return Outcome.Failure<string>(validation.Error);

            var relativePath = GenerateFilePath(fileName, container);
            var fullPath = Path.Combine(_options.RootPath, relativePath);

            var directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            using var output = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
            await fileStream.CopyToAsync(output);

            _logger.LogInformation("File uploaded: {Path}", relativePath);
            return Outcome<string>.Successful(relativePath);
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

    // ============================================================
    // IFileDownloader
    // ============================================================
    public Task<Outcome<Stream>> DownloadAsync(string filePath)
    {
        try
        {
            var fullPath = GetFullPath(filePath);
            if (!File.Exists(fullPath))
                return Task.FromResult(Outcome.Failure<Stream>(new OutcomeError("File not found.", "FILE_NOT_FOUND", OutcomeErrorType.NotFound)));

            var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            return Task.FromResult(Outcome.Successful<Stream>(stream));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading file: {Path}", filePath);
            return Task.FromResult(Outcome.Failure<Stream>(new OutcomeError(ex.Message, "FILE_DOWNLOAD_ERROR", OutcomeErrorType.Failure)));
        }
    }

    public async Task<Outcome<byte[]>> DownloadBytesAsync(string filePath)
    {
        var result = await DownloadAsync(filePath);
        if (!result.IsSuccess)
            return Outcome.Failure<byte[]>(result.Error);

        try
        {
            using var memory = new MemoryStream();
            await result.Data.CopyToAsync(memory);
            return Outcome<byte[]>.Successful(memory.ToArray());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading bytes: {Path}", filePath);
            return Outcome.Failure<byte[]>(new OutcomeError(ex.Message, "FILE_READ_ERROR", OutcomeErrorType.Failure));
        }
        finally
        {
            result.Data?.Dispose();
        }
    }

    // ============================================================
    // GetUrlAsync
    // ============================================================
    public Task<Outcome<string>> GetUrlAsync(string filePath)
    {
        var url = $"{_options.UrlBasePath.TrimEnd('/')}/{filePath.Replace('\\', '/')}";
        return Task.FromResult(Outcome<string>.Successful(url));
    }

    // ============================================================
    // Helpers
    // ============================================================
    private string GetFullPath(string relativePath)
    {
        var full = Path.GetFullPath(Path.Combine(_options.RootPath, relativePath));
        var root = Path.GetFullPath(_options.RootPath);

        if (!full.StartsWith(root, StringComparison.OrdinalIgnoreCase))
            throw new SecurityException("Invalid path traversal attempt.");

        return full;
    }

    private string GenerateFilePath(string fileName, string? container)
    {
        var extension = Path.GetExtension(fileName);
        var uniqueName = $"{Guid.NewGuid():N}{extension}";
        var path = string.Empty;

        if (_options.UseDateSubdirectories)
        {
            var now = DateTime.Now;
            path = Path.Combine(now.ToString("yyyy"), now.ToString("MM"), now.ToString("dd"));
        }

        if (!string.IsNullOrEmpty(container))
            path = Path.Combine(path, container);

        return Path.Combine(path, uniqueName);
    }

    private Outcome ValidateFile(string fileName, long fileSize)
    {
        if (fileSize > _options.MaxFileSize)
        {
            var sizeInMB = _options.MaxFileSize / (1024 * 1024);
            return Outcome.Failure(new OutcomeError(
                $"File size exceeds maximum allowed ({sizeInMB} MB).",
                "FILE_TOO_LARGE", OutcomeErrorType.Validation));
        }

        if (_options.AllowedExtensions.Length > 0)
        {
            var extension = Path.GetExtension(fileName);
            if (!_options.AllowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
            {
                return Outcome.Failure(new OutcomeError(
                    $"File extension '{extension}' is not allowed.",
                    "FILE_EXTENSION_NOT_ALLOWED", OutcomeErrorType.Validation));
            }
        }

        return Outcome.Successful();
    }
}