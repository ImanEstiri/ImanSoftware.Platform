using ImanSoftware.Outcomes;

namespace ImanSoftware.FileStorage;

/// <summary>
/// Contract for directory and path-level operations.
/// </summary>
public interface IDirectoryOperator
{
    Outcome<bool> FileExists(string path);
    Outcome<bool> DirectoryExists(string path);

    Outcome CreateDirectory(string path);
    Outcome DeleteDirectory(string path, bool recursive = false);

    /// <summary>
    /// Enumerates files matching a single search pattern.
    /// </summary>
    Outcome<IEnumerable<string>> EnumerateFiles(
        string path,
        string searchPattern = "*",
        SearchOption searchOption = SearchOption.TopDirectoryOnly);

    /// <summary>
    /// Enumerates files matching any of the given search patterns.
    /// Example: EnumerateFiles(path, new[] { "*.log", "*.txt" }, SearchOption.AllDirectories)
    /// </summary>
    Outcome<IEnumerable<string>> EnumerateFiles(
        string path,
        IEnumerable<string> searchPatterns,
        SearchOption searchOption = SearchOption.TopDirectoryOnly);

    /// <summary>
    /// Convenience method: finds files by extension(s).
    /// Example: FindFiles(path, includeSubdirectories: true, ".log", ".txt")
    /// </summary>
    Outcome<IEnumerable<string>> FindFiles(
        string path,
        bool includeSubdirectories,
        params string[] extensions);

    Outcome<IEnumerable<string>> EnumerateDirectories(
        string path,
        string searchPattern = "*",
        SearchOption searchOption = SearchOption.TopDirectoryOnly);
}