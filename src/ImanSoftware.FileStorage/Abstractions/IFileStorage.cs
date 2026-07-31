// مسیر: Abstractions/FileStorage/IFileStorage.cs
namespace ImanSoftware.FileStorage;

/// <summary>
/// Combined contract for full file storage operations.
/// Inherits all individual file operation interfaces.
/// </summary>
public interface IFileStorage :
    IFileUploader,
    IFileDownloader,
    IFileDeleter,
    IFileInfoProvider,
    IFileMover
{
    // No additional methods needed - just composition
}