using ImanSoftware.Outcomes;

namespace ImanSoftware.FileStorage;

/// <summary>
/// Contract for moving and copying files.
/// </summary>
public interface IFileMover
{
    Task<Outcome> MoveAsync(string sourcePath, string destinationPath);
    Task<Outcome> CopyAsync(string sourcePath, string destinationPath);
}