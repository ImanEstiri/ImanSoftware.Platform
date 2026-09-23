using ImanSoftware.Outcomes;

namespace ImanSoftware.FileStorage;

/// <summary>
/// Contract for upload/download operations that require a configured storage backend.
/// </summary>
public interface IFileTransfer :
    IFileUploader,
    IFileDownloader
{
    /// <summary>
    /// Gets a public URL to access the file.
    /// </summary>
    Task<Outcome<string>> GetUrlAsync(string filePath);
}