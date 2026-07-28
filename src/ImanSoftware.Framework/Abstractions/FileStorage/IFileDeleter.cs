// مسیر: Abstractions/FileStorage/IFileDeleter.cs
using ImanSoftware.Framework.Core.Results;

namespace ImanSoftware.Framework.Abstractions.FileStorage;

/// <summary>
/// Contract for deleting files from a storage system.
/// </summary>
public interface IFileDeleter
{
    /// <summary>
    /// Deletes a file from the storage.
    /// </summary>
    Task<Result> DeleteAsync(string filePath);
}