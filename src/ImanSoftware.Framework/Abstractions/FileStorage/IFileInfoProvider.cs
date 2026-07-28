// مسیر: Abstractions/FileStorage/IFileInfoProvider.cs
using ImanSoftware.Framework.Core.FileStorage;
using ImanSoftware.Framework.Core.Results;

namespace ImanSoftware.Framework.Abstractions.FileStorage;

/// <summary>
/// Contract for retrieving file information and metadata.
/// </summary>
public interface IFileInfoProvider
{
    /// <summary>
    /// Checks if a file exists in the storage.
    /// </summary>
    Task<Result<bool>> ExistsAsync(string filePath);

    /// <summary>
    /// Retrieves metadata information of a file.
    /// </summary>
    Task<Result<FileMetadata>> GetMetadataAsync(string filePath);

    /// <summary>
    /// Gets a public URL to access the file (if supported).
    /// </summary>
    Task<Result<string>> GetUrlAsync(string filePath);
}