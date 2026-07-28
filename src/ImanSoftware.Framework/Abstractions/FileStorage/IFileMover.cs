// مسیر: Abstractions/FileStorage/IFileMover.cs
using ImanSoftware.Framework.Core.Results;

namespace ImanSoftware.Framework.Abstractions.FileStorage;

/// <summary>
/// Contract for moving and copying files.
/// </summary>
public interface IFileMover
{
    /// <summary>
    /// Moves a file from source to destination.
    /// </summary>
    Task<Result> MoveAsync(string sourcePath, string destinationPath);

    /// <summary>
    /// Copies a file from source to destination.
    /// </summary>
    Task<Result> CopyAsync(string sourcePath, string destinationPath);
}