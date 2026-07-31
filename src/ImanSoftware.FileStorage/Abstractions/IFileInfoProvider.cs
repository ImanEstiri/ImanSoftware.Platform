// مسیر: Abstractions/FileStorage/IFileInfoProvider.cs
using ImanSoftware.FileStorage;
using ImanSoftware.Outcomes;

namespace ImanSoftware.FileStorage;

/// <summary>
/// Contract for retrieving file information and metadata.
/// </summary>
public interface IFileInfoProvider
{
    /// <summary>
    /// Checks if a file exists in the storage.
    /// </summary>
    Task<Outcome<bool>> ExistsAsync(string filePath);

    /// <summary>
    /// Retrieves metadata information of a file.
    /// </summary>
    Task<Outcome<FileMetadata>> GetMetadataAsync(string filePath);

    /// <summary>
    /// Gets a public URL to access the file (if supported).
    /// </summary>
    Task<Outcome<string>> GetUrlAsync(string filePath);
}