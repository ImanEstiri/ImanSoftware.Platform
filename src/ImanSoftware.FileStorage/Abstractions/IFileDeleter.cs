// مسیر: Abstractions/FileStorage/IFileDeleter.cs
using ImanSoftware.Outcomes;

namespace ImanSoftware.FileStorage;

/// <summary>
/// Contract for deleting files from a storage system.
/// </summary>
public interface IFileDeleter
{
    /// <summary>
    /// Deletes a file from the storage.
    /// </summary>
    Task<Outcome> DeleteAsync(string filePath);
}