using ImanSoftware.Outcomes;

namespace ImanSoftware.FileStorage;

/// <summary>
/// Contract for directory and path-level operations.
/// No settings needed - paths are used directly.
/// </summary>
public interface IDirectoryOperator
{
    Outcome<bool> FileExists(string path);
    Outcome<bool> DirectoryExists(string path);

    Outcome CreateDirectory(string path);
    Outcome DeleteDirectory(string path, bool recursive = false);

    Outcome<IEnumerable<string>> EnumerateFiles(
        string path,
        string searchPattern = "*",
        SearchOption searchOption = SearchOption.TopDirectoryOnly);

    Outcome<IEnumerable<string>> EnumerateDirectories(
        string path,
        string searchPattern = "*",
        SearchOption searchOption = SearchOption.TopDirectoryOnly);
}