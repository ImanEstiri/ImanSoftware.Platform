using ImanSoftware.FileStorage;
using ImanSoftware.Logging;
using ImanSoftware.Outcomes;

namespace ImanSoftware.FileStorage;

/// <summary>
/// General-purpose file storage using the local file system.
/// Works with any path (absolute or relative) - no settings required.
/// </summary>
public class LocalFileStorage : IFileStorage
{
    private readonly ILoggerAdapter<LocalFileStorage> _logger;

    public LocalFileStorage(ILoggerAdapter<LocalFileStorage> logger)
    {
        _logger = logger;
    }

    // ============================================================
    // IFileReader
    // ============================================================
    public async Task<Outcome<string>> ReadAllTextAsync(string path, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!File.Exists(path))
                return Outcome.Failure<string>(new OutcomeError("File not found.", "FILE_NOT_FOUND", OutcomeErrorType.NotFound));

            var content = await File.ReadAllTextAsync(path, cancellationToken);
            return Outcome<string>.Successful(content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading text file: {Path}", path);
            return Outcome.Failure<string>(new OutcomeError(ex.Message, "FILE_READ_ERROR", OutcomeErrorType.Failure));
        }
    }

    public async Task<Outcome<string[]>> ReadAllLinesAsync(string path, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!File.Exists(path))
                return Outcome.Failure<string[]>(new OutcomeError("File not found.", "FILE_NOT_FOUND", OutcomeErrorType.NotFound));

            var lines = await File.ReadAllLinesAsync(path, cancellationToken);
            return Outcome<string[]>.Successful(lines);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading lines from file: {Path}", path);
            return Outcome.Failure<string[]>(new OutcomeError(ex.Message, "FILE_READ_ERROR", OutcomeErrorType.Failure));
        }
    }

    public async Task<Outcome<byte[]>> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!File.Exists(path))
                return Outcome.Failure<byte[]>(new OutcomeError("File not found.", "FILE_NOT_FOUND", OutcomeErrorType.NotFound));

            var bytes = await File.ReadAllBytesAsync(path, cancellationToken);
            return Outcome<byte[]>.Successful(bytes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading bytes from file: {Path}", path);
            return Outcome.Failure<byte[]>(new OutcomeError(ex.Message, "FILE_READ_ERROR", OutcomeErrorType.Failure));
        }
    }

    public async IAsyncEnumerable<string> ReadLinesAsync(
        string path,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var reader = new StreamReader(path);
        while (!reader.EndOfStream)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var line = await reader.ReadLineAsync(cancellationToken);
            if (line is not null)
                yield return line;
        }
    }

    // ============================================================
    // IFileWriter
    // ============================================================
    public async Task<Outcome> WriteAllTextAsync(string path, string content, CancellationToken cancellationToken = default)
    {
        try
        {
            EnsureDirectory(Path.GetDirectoryName(path));
            await File.WriteAllTextAsync(path, content, cancellationToken);
            return Outcome.Successful();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error writing text file: {Path}", path);
            return Outcome.Failure(new OutcomeError(ex.Message, "FILE_WRITE_ERROR", OutcomeErrorType.Failure));
        }
    }

    public async Task<Outcome> WriteAllLinesAsync(string path, IEnumerable<string> lines, CancellationToken cancellationToken = default)
    {
        try
        {
            EnsureDirectory(Path.GetDirectoryName(path));
            await File.WriteAllLinesAsync(path, lines, cancellationToken);
            return Outcome.Successful();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error writing lines to file: {Path}", path);
            return Outcome.Failure(new OutcomeError(ex.Message, "FILE_WRITE_ERROR", OutcomeErrorType.Failure));
        }
    }

    public async Task<Outcome> WriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken = default)
    {
        try
        {
            EnsureDirectory(Path.GetDirectoryName(path));
            await File.WriteAllBytesAsync(path, bytes, cancellationToken);
            return Outcome.Successful();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error writing bytes to file: {Path}", path);
            return Outcome.Failure(new OutcomeError(ex.Message, "FILE_WRITE_ERROR", OutcomeErrorType.Failure));
        }
    }

    public async Task<Outcome> AppendAllTextAsync(string path, string content, CancellationToken cancellationToken = default)
    {
        try
        {
            EnsureDirectory(Path.GetDirectoryName(path));
            await File.AppendAllTextAsync(path, content, cancellationToken);
            return Outcome.Successful();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error appending to file: {Path}", path);
            return Outcome.Failure(new OutcomeError(ex.Message, "FILE_WRITE_ERROR", OutcomeErrorType.Failure));
        }
    }

    // ============================================================
    // IFileDeleter
    // ============================================================
    public Task<Outcome> DeleteAsync(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
                return Task.FromResult(Outcome.Failure(new OutcomeError("File not found.", "FILE_NOT_FOUND", OutcomeErrorType.NotFound)));

            File.Delete(filePath);
            return Task.FromResult(Outcome.Successful());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file: {Path}", filePath);
            return Task.FromResult(Outcome.Failure(new OutcomeError(ex.Message, "FILE_DELETE_ERROR", OutcomeErrorType.Failure)));
        }
    }

    // ============================================================
    // IFileMover
    // ============================================================
    public Task<Outcome> MoveAsync(string sourcePath, string destinationPath)
    {
        try
        {
            if (!File.Exists(sourcePath))
                return Task.FromResult(Outcome.Failure(new OutcomeError("Source file not found.", "FILE_NOT_FOUND", OutcomeErrorType.NotFound)));

            EnsureDirectory(Path.GetDirectoryName(destinationPath));
            File.Move(sourcePath, destinationPath);
            return Task.FromResult(Outcome.Successful());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moving file: {Src} → {Dst}", sourcePath, destinationPath);
            return Task.FromResult(Outcome.Failure(new OutcomeError(ex.Message, "FILE_MOVE_ERROR", OutcomeErrorType.Failure)));
        }
    }

    public Task<Outcome> CopyAsync(string sourcePath, string destinationPath)
    {
        try
        {
            if (!File.Exists(sourcePath))
                return Task.FromResult(Outcome.Failure(new OutcomeError("Source file not found.", "FILE_NOT_FOUND", OutcomeErrorType.NotFound)));

            EnsureDirectory(Path.GetDirectoryName(destinationPath));
            File.Copy(sourcePath, destinationPath, overwrite: true);
            return Task.FromResult(Outcome.Successful());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error copying file: {Src} → {Dst}", sourcePath, destinationPath);
            return Task.FromResult(Outcome.Failure(new OutcomeError(ex.Message, "FILE_COPY_ERROR", OutcomeErrorType.Failure)));
        }
    }

    // ============================================================
    // IFileInfoProvider
    // ============================================================
    public Task<Outcome<FileMetadata>> GetMetadataAsync(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
                return Task.FromResult(Outcome.Failure<FileMetadata>(new OutcomeError("File not found.", "FILE_NOT_FOUND", OutcomeErrorType.NotFound)));

            var fileInfo = new FileInfo(filePath);
            var metadata = new FileMetadata
            {
                FileName = fileInfo.Name,
                Size = fileInfo.Length,
                ContentType = GetContentType(fileInfo.Extension),
                CreatedAt = fileInfo.CreationTime,
                LastModifiedAt = fileInfo.LastWriteTime
            };

            return Task.FromResult(Outcome.Successful<FileMetadata>(metadata));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting metadata: {Path}", filePath);
            return Task.FromResult(Outcome.Failure<FileMetadata>(new OutcomeError(ex.Message, "FILE_METADATA_ERROR", OutcomeErrorType.Failure)));
        }
    }

    // ============================================================
    // IDirectoryOperator
    // ============================================================
    public Outcome<bool> FileExists(string path)
    {
        try { return Outcome<bool>.Successful(File.Exists(path)); }
        catch (Exception ex) { return Outcome.Failure<bool>(new OutcomeError(ex.Message, "FILE_EXISTS_ERROR", OutcomeErrorType.Failure)); }
    }

    public Outcome<bool> DirectoryExists(string path)
    {
        try { return Outcome<bool>.Successful(Directory.Exists(path)); }
        catch (Exception ex) { return Outcome.Failure<bool>(new OutcomeError(ex.Message, "DIR_EXISTS_ERROR", OutcomeErrorType.Failure)); }
    }

    public Outcome CreateDirectory(string path)
    {
        try
        {
            Directory.CreateDirectory(path);
            return Outcome.Successful();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating directory: {Path}", path);
            return Outcome.Failure(new OutcomeError(ex.Message, "DIR_CREATE_ERROR", OutcomeErrorType.Failure));
        }
    }

    public Outcome DeleteDirectory(string path, bool recursive = false)
    {
        try
        {
            if (!Directory.Exists(path))
                return Outcome.Failure(new OutcomeError("Directory not found.", "DIR_NOT_FOUND", OutcomeErrorType.NotFound));

            Directory.Delete(path, recursive);
            return Outcome.Successful();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting directory: {Path}", path);
            return Outcome.Failure(new OutcomeError(ex.Message, "DIR_DELETE_ERROR", OutcomeErrorType.Failure));
        }
    }

    public Outcome<IEnumerable<string>> EnumerateFiles(
        string path,
        string searchPattern = "*",
        SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        try
        {
            if (!Directory.Exists(path))
                return Outcome.Failure<IEnumerable<string>>(new OutcomeError("Directory not found.", "DIR_NOT_FOUND", OutcomeErrorType.NotFound));

            return Outcome<IEnumerable<string>>.Successful(
                Directory.EnumerateFiles(path, searchPattern, searchOption));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enumerating files in: {Path}", path);
            return Outcome.Failure<IEnumerable<string>>(new OutcomeError(ex.Message, "DIR_ENUM_ERROR", OutcomeErrorType.Failure));
        }
    }

    public Outcome<IEnumerable<string>> EnumerateDirectories(
        string path,
        string searchPattern = "*",
        SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        try
        {
            if (!Directory.Exists(path))
                return Outcome.Failure<IEnumerable<string>>(new OutcomeError("Directory not found.", "DIR_NOT_FOUND", OutcomeErrorType.NotFound));

            return Outcome<IEnumerable<string>>.Successful(
                Directory.EnumerateDirectories(path, searchPattern, searchOption));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enumerating directories in: {Path}", path);
            return Outcome.Failure<IEnumerable<string>>(new OutcomeError(ex.Message, "DIR_ENUM_ERROR", OutcomeErrorType.Failure));
        }
    }

    // ============================================================
    // Helpers
    // ============================================================
    private static void EnsureDirectory(string? directory)
    {
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);
    }

    private static string GetContentType(string extension) => extension.ToLowerInvariant() switch
    {
        ".jpg" or ".jpeg" => "image/jpeg",
        ".png" => "image/png",
        ".gif" => "image/gif",
        ".pdf" => "application/pdf",
        ".txt" => "text/plain",
        ".json" => "application/json",
        ".xml" => "application/xml",
        _ => "application/octet-stream"
    };
}