using ImanSoftware.FileStorage.Infrastructure;

namespace ImanSoftware.FileStorage;

/// <summary>
/// Factory for creating file storage instances without dependency injection.
/// Use this in simple scenarios (console apps, scripts, small utilities).
/// For DI scenarios, use AddImanFileStorage() / AddImanFileTransfer().
/// </summary>
public static class ImanFileStorageFactory
{
    /// <summary>
    /// Creates a general-purpose file storage instance (no configuration needed).
    /// </summary>
    public static IFileStorage CreateStorage()
    {
        return new LocalFileStorage();
    }

    /// <summary>
    /// Creates an upload/download file transfer instance with the given options.
    /// </summary>
    public static IFileTransfer CreateTransfer(Action<FileTransferOptions>? configure = null)
    {
        var options = new FileTransferOptions();
        configure?.Invoke(options);
        return new LocalFileTransfer(options);
    }

    /// <summary>
    /// Creates an upload/download file transfer instance with explicit options.
    /// </summary>
    public static IFileTransfer CreateTransfer(FileTransferOptions options)
    {
        return new LocalFileTransfer(options);
    }
}