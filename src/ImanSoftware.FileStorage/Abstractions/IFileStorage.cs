namespace ImanSoftware.FileStorage;

/// <summary>
/// General-purpose file storage operations.
/// Works with any file path - no configuration required.
/// Use <see cref="IFileTransfer"/> for upload/download operations.
/// </summary>
public interface IFileStorage :
    IFileReader,
    IFileWriter,
    IFileDeleter,
    IFileMover,
    IFileInfoProvider,
    IDirectoryOperator
{
}