using ImanSoftware.Outcomes;

namespace ImanSoftware.FileStorage;

/// <summary>
/// Contract for uploading files to a configured storage backend.
/// </summary>
public interface IFileUploader
{
    Task<Outcome<string>> UploadAsync(Stream fileStream, string fileName, string? container = null);
    Task<Outcome<string>> UploadAsync(byte[] fileBytes, string fileName, string? container = null);
}