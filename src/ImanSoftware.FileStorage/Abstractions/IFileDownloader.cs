using ImanSoftware.Outcomes;

namespace ImanSoftware.FileStorage;

/// <summary>
/// Contract for downloading files from a configured storage backend.
/// </summary>
public interface IFileDownloader
{
    Task<Outcome<Stream>> DownloadAsync(string filePath);
    Task<Outcome<byte[]>> DownloadBytesAsync(string filePath);
}