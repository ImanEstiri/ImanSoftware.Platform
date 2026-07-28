// مسیر: Abstractions/FileStorage/IFileUploader.cs
using ImanSoftware.Framework.Core.Results;

namespace ImanSoftware.Framework.Abstractions.FileStorage;

/// <summary>
/// Contract for uploading files to a storage system.
/// </summary>
public interface IFileUploader
{
    /// <summary>
    /// Uploads a file from a stream.
    /// </summary>
    /// <param name="fileStream">The file content as a stream.</param>
    /// <param name="fileName">The name of the file (including extension).</param>
    /// <param name="container">Optional container/folder name.</param>
    /// <returns>The path or identifier of the stored file.</returns>
    Task<Result<string>> UploadAsync(Stream fileStream, string fileName, string? container = null);

    /// <summary>
    /// Uploads a file from a byte array.
    /// </summary>
    Task<Result<string>> UploadAsync(byte[] fileBytes, string fileName, string? container = null);
}