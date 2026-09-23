using ImanSoftware.Outcomes;

namespace ImanSoftware.FileStorage;

/// <summary>
/// Contract for deleting files.
/// </summary>
public interface IFileDeleter
{
    Task<Outcome> DeleteAsync(string filePath);
}