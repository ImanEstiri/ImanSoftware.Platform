using ImanSoftware.Outcomes;

namespace ImanSoftware.FileStorage;

/// <summary>
/// Contract for retrieving file metadata.
/// </summary>
public interface IFileInfoProvider
{
    Task<Outcome<FileMetadata>> GetMetadataAsync(string filePath);
}