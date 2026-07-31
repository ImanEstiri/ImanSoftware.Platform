// مسیر: Abstractions/FileStorage/IFileMover.cs
using ImanSoftware.Outcomes;

namespace ImanSoftware.FileStorage;

/// <summary>
/// Contract for moving and copying files.
/// </summary>
public interface IFileMover
{
    /// <summary>
    /// Moves a file from source to destination.
    /// </summary>
    Task<Outcome> MoveAsync(string sourcePath, string destinationPath);

    /// <summary>
    /// Copies a file from source to destination.
    /// </summary>
    Task<Outcome> CopyAsync(string sourcePath, string destinationPath);
}