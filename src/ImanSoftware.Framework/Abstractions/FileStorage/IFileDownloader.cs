// مسیر: Abstractions/FileStorage/IFileDownloader.cs
using ImanSoftware.Framework.Core.Results;

namespace ImanSoftware.Framework.Abstractions.FileStorage;

/// <summary>
/// Contract for downloading files from a storage system.
/// </summary>
public interface IFileDownloader
{
    /// <summary>
    /// Downloads a file as a stream.
    /// </summary>
    Task<Result<Stream>> DownloadAsync(string filePath);

    /// <summary>
    /// Downloads a file as a byte array.
    /// </summary>
    Task<Result<byte[]>> DownloadBytesAsync(string filePath);
}