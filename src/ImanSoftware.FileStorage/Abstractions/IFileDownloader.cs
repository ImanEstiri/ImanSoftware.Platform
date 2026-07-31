// مسیر: Abstractions/FileStorage/IFileDownloader.cs
using ImanSoftware.Outcomes;

namespace ImanSoftware.FileStorage;

/// <summary>
/// Contract for downloading files from a storage system.
/// </summary>
public interface IFileDownloader
{
    /// <summary>
    /// Downloads a file as a stream.
    /// </summary>
    Task<Outcome<Stream>> DownloadAsync(string filePath);

    /// <summary>
    /// Downloads a file as a byte array.
    /// </summary>
    Task<Outcome<byte[]>> DownloadBytesAsync(string filePath);
}