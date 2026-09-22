using ImanSoftware.Outcomes;

namespace ImanSoftware.FileStorage;

/// <summary>
/// Contract for upload/download operations that require a configured storage backend.
/// Use <see cref="IFileStorage"/> for general file operations.
/// </summary>
public interface IFileTransfer :
    IFileUploader,
    IFileDownloader
{
    /// <summary>
    /// Gets a public URL to access the file.
    /// Requires the storage backend to have a configured URL base path.
    /// </summary>
    Task<Outcome<string>> GetUrlAsync(string filePath);
}