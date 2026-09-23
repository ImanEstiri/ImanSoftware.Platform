using System.Runtime.CompilerServices;
using ImanSoftware.Outcomes;

namespace ImanSoftware.FileStorage.Infrastructure;

/// <summary>
/// General-purpose file storage using the local file system.
/// Works with any path (absolute or relative) - no configuration required.
/// </summary>
internal sealed class LocalFileStorage : IFileStorage
{
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
            return Outcome.Failure<byte[]>(new OutcomeError(ex.Message, "FILE_READ_ERROR", OutcomeErrorType.Failure));
        }
    }

    public async IAsyncEnumerable<string> ReadLinesAsync(
        string path,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
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
                return Task.FromResult(Outcome.Failure(
                    new OutcomeError("File not found.", "FILE_NOT_FOUND", OutcomeErrorType.NotFound)));

            File.Delete(filePath);
            return Task.FromResult(Outcome.Successful());
        }
        catch (Exception ex)
        {
            return Task.FromResult(Outcome.Failure(
                new OutcomeError(ex.Message, "FILE_DELETE_ERROR", OutcomeErrorType.Failure)));
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
                return Task.FromResult(Outcome.Failure(
                    new OutcomeError("Source file not found.", "FILE_NOT_FOUND", OutcomeErrorType.NotFound)));

            EnsureDirectory(Path.GetDirectoryName(destinationPath));
            File.Move(sourcePath, destinationPath);
            return Task.FromResult(Outcome.Successful());
        }
        catch (Exception ex)
        {
            return Task.FromResult(Outcome.Failure(
                new OutcomeError(ex.Message, "FILE_MOVE_ERROR", OutcomeErrorType.Failure)));
        }
    }

    public Task<Outcome> CopyAsync(string sourcePath, string destinationPath)
    {
        try
        {
            if (!File.Exists(sourcePath))
                return Task.FromResult(Outcome.Failure(
                    new OutcomeError("Source file not found.", "FILE_NOT_FOUND", OutcomeErrorType.NotFound)));

            EnsureDirectory(Path.GetDirectoryName(destinationPath));
            File.Copy(sourcePath, destinationPath, overwrite: true);
            return Task.FromResult(Outcome.Successful());
        }
        catch (Exception ex)
        {
            return Task.FromResult(Outcome.Failure(
                new OutcomeError(ex.Message, "FILE_COPY_ERROR", OutcomeErrorType.Failure)));
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
                return Task.FromResult(Outcome.Failure<FileMetadata>(
                    new OutcomeError("File not found.", "FILE_NOT_FOUND", OutcomeErrorType.NotFound)));

            var fileInfo = new FileInfo(filePath);
            var metadata = new FileMetadata
            {
                FullPath = fileInfo.FullName,
                FileName = fileInfo.Name,
                Extension = fileInfo.Extension,
                Size = fileInfo.Length,
                ContentType = GetContentType(fileInfo.Extension),
                CreatedAt = fileInfo.CreationTime,
                LastModifiedAt = fileInfo.LastWriteTime
            };

            return Task.FromResult(Outcome.Successful<FileMetadata>(metadata));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Outcome.Failure<FileMetadata>(
                new OutcomeError(ex.Message, "FILE_METADATA_ERROR", OutcomeErrorType.Failure)));
        }
    }

    // ============================================================
    // IDirectoryOperator - Existence checks
    // ============================================================

    public bool FileExists(string path)
    {
        try { return File.Exists(path); }
        catch { return false; }
    }

    public bool DirectoryExists(string path)
    {
        try { return Directory.Exists(path); }
        catch { return false; }
    }

    // ============================================================
    // IDirectoryOperator - Directory operations
    // ============================================================

    public Task<Outcome> CreateDirectoryAsync(string path, CancellationToken cancellationToken = default)
    {
        try
        {
            Directory.CreateDirectory(path);
            return Task.FromResult(Outcome.Successful());
        }
        catch (Exception ex)
        {
            return Task.FromResult(Outcome.Failure(
                new OutcomeError(ex.Message, "DIR_CREATE_ERROR", OutcomeErrorType.Failure)));
        }
    }

    public Task<Outcome> DeleteDirectoryAsync(string path, bool recursive = false, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!Directory.Exists(path))
                return Task.FromResult(Outcome.Failure(
                    new OutcomeError("Directory not found.", "DIR_NOT_FOUND", OutcomeErrorType.NotFound)));

            Directory.Delete(path, recursive);
            return Task.FromResult(Outcome.Successful());
        }
        catch (Exception ex)
        {
            return Task.FromResult(Outcome.Failure(
                new OutcomeError(ex.Message, "DIR_DELETE_ERROR", OutcomeErrorType.Failure)));
        }
    }

    // ============================================================
    // IDirectoryOperator - Enumerate files
    // ============================================================

    public Task<Outcome<IEnumerable<string>>> EnumerateFilesAsync(
        string path,
        IEnumerable<string> searchPatterns,
        SearchOption searchOption = SearchOption.TopDirectoryOnly,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!Directory.Exists(path))
                return Task.FromResult(Outcome.Failure<IEnumerable<string>>(
                    new OutcomeError("Directory not found.", "DIR_NOT_FOUND", OutcomeErrorType.NotFound)));

            var patterns = NormalizePatterns(searchPatterns);

            // No pattern (or single "*"): enumerate all files - fastest path
            if (patterns.Length == 1 && patterns[0] == "*")
            {
                var allFiles = Directory.EnumerateFiles(path, "*", searchOption);
                return Task.FromResult(Outcome<IEnumerable<string>>.Successful(allFiles));
            }

            // Single pattern: use built-in
            if (patterns.Length == 1)
            {
                var files = Directory.EnumerateFiles(path, patterns[0], searchOption);
                return Task.FromResult(Outcome<IEnumerable<string>>.Successful(files));
            }

            // Multiple patterns: enumerate all then filter
            var filtered = Directory.EnumerateFiles(path, "*", searchOption)
                .Where(file => patterns.Any(p => MatchesPattern(file, p)));

            return Task.FromResult(Outcome<IEnumerable<string>>.Successful(filtered));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Outcome.Failure<IEnumerable<string>>(
                new OutcomeError(ex.Message, "DIR_ENUM_ERROR", OutcomeErrorType.Failure)));
        }
    }

    // ============================================================
    // IDirectoryOperator - Enumerate directories
    // ============================================================

    public Task<Outcome<IEnumerable<string>>> EnumerateDirectoriesAsync(
        string path,
        string searchPattern = "*",
        SearchOption searchOption = SearchOption.TopDirectoryOnly,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!Directory.Exists(path))
                return Task.FromResult(Outcome.Failure<IEnumerable<string>>(
                    new OutcomeError("Directory not found.", "DIR_NOT_FOUND", OutcomeErrorType.NotFound)));

            var directories = Directory.EnumerateDirectories(path, searchPattern, searchOption);
            return Task.FromResult(Outcome<IEnumerable<string>>.Successful(directories));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Outcome.Failure<IEnumerable<string>>(
                new OutcomeError(ex.Message, "DIR_ENUM_ERROR", OutcomeErrorType.Failure)));
        }
    }

    // ============================================================
    // IDirectoryOperator - Search files with metadata
    // ============================================================

    public Task<Outcome<IEnumerable<FileMetadata>>> SearchFilesAsync(
        string path,
        IEnumerable<string> searchPatterns,
        SearchOption searchOption = SearchOption.TopDirectoryOnly,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!Directory.Exists(path))
                return Task.FromResult(Outcome.Failure<IEnumerable<FileMetadata>>(
                    new OutcomeError("Directory not found.", "DIR_NOT_FOUND", OutcomeErrorType.NotFound)));

            var patterns = NormalizePatterns(searchPatterns);

            // Determine which files to include
            IEnumerable<string> matchedFiles;
            if (patterns.Length == 1 && patterns[0] == "*")
            {
                matchedFiles = Directory.EnumerateFiles(path, "*", searchOption);
            }
            else if (patterns.Length == 1)
            {
                matchedFiles = Directory.EnumerateFiles(path, patterns[0], searchOption);
            }
            else
            {
                matchedFiles = Directory.EnumerateFiles(path, "*", searchOption)
                    .Where(file => patterns.Any(p => MatchesPattern(file, p)));
            }

            // Build metadata list
            var metadataList = new List<FileMetadata>();

            foreach (var file in matchedFiles)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    var fileInfo = new FileInfo(file);
                    metadataList.Add(new FileMetadata
                    {
                        FullPath = fileInfo.FullName,
                        FileName = fileInfo.Name,
                        Extension = fileInfo.Extension,
                        Size = fileInfo.Length,
                        ContentType = GetContentType(fileInfo.Extension),
                        CreatedAt = fileInfo.CreationTime,
                        LastModifiedAt = fileInfo.LastWriteTime
                    });
                }
                catch
                {
                    // Skip files that cannot be accessed (permission, deleted, etc.)
                }
            }

            return Task.FromResult(Outcome.Successful<IEnumerable<FileMetadata>>(metadataList));
        }
        catch (Exception ex)
        {
            return Task.FromResult(Outcome.Failure<IEnumerable<FileMetadata>>(
                new OutcomeError(ex.Message, "FILE_SEARCH_ERROR", OutcomeErrorType.Failure)));
        }
    }

    // ============================================================
    // Private Helpers
    // ============================================================

    /// <summary>
    /// Ensures the parent directory exists before writing.
    /// </summary>
    private static void EnsureDirectory(string? directory)
    {
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);
    }

    /// <summary>
    /// Normalizes the input patterns: trims, removes empty entries, defaults to "*" if empty.
    /// </summary>
    private static string[] NormalizePatterns(IEnumerable<string>? patterns)
    {
        var result = patterns?
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Select(p => p.Trim())
            .Distinct()
            .ToArray()
            ?? Array.Empty<string>();

        return result.Length == 0 ? new[] { "*" } : result;
    }

    /// <summary>
    /// Matches a file path against a wildcard pattern (e.g., "*.log").
    /// Uses .NET's built-in FileSystemName matcher (fast, no regex).
    /// </summary>
    private static bool MatchesPattern(string filePath, string pattern)
    {
        var fileName = Path.GetFileName(filePath);

        return System.IO.Enumeration.FileSystemName.MatchesSimpleExpression(
            pattern,
            fileName,
            ignoreCase: true);
    }

    /// <summary>
    /// Gets the MIME content type based on the file extension.
    /// </summary>
    private static string GetContentType(string extension) => extension.ToLowerInvariant() switch
    {
        ".jpg" or ".jpeg" => "image/jpeg",
        ".png" => "image/png",
        ".gif" => "image/gif",
        ".bmp" => "image/bmp",
        ".svg" => "image/svg+xml",
        ".webp" => "image/webp",
        ".ico" => "image/x-icon",
        ".pdf" => "application/pdf",
        ".txt" => "text/plain",
        ".json" => "application/json",
        ".xml" => "application/xml",
        ".yaml" or ".yml" => "application/x-yaml",
        ".html" or ".htm" => "text/html",
        ".css" => "text/css",
        ".js" => "application/javascript",
        ".ts" => "application/typescript",
        ".csv" => "text/csv",
        ".md" => "text/markdown",
        ".zip" => "application/zip",
        ".rar" => "application/x-rar-compressed",
        ".7z" => "application/x-7z-compressed",
        ".tar" => "application/x-tar",
        ".gz" => "application/gzip",
        ".doc" => "application/msword",
        ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        ".xls" => "application/vnd.ms-excel",
        ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        ".ppt" => "application/vnd.ms-powerpoint",
        ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
        ".mp3" => "audio/mpeg",
        ".mp4" => "video/mp4",
        ".avi" => "video/x-msvideo",
        ".mov" => "video/quicktime",
        ".wav" => "audio/wav",
        ".woff" => "font/woff",
        ".woff2" => "font/woff2",
        ".ttf" => "font/ttf",
        ".otf" => "font/otf",
        _ => "application/octet-stream"
    };
}