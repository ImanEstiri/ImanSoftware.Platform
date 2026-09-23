using ImanSoftware.Outcomes;

namespace ImanSoftware.FileStorage;

/// <summary>
/// Contract for directory and path-level operations.
/// </summary>
public interface IDirectoryOperator
{
    // ============================================================
    // Instant checks (no I/O wait)
    // ============================================================

    /// <summary>
    /// Checks if a file exists. Returns false on any error (permission, invalid path, etc.).
    /// </summary>
    bool FileExists(string path);

    /// <summary>
    /// Checks if a directory exists. Returns false on any error.
    /// </summary>
    bool DirectoryExists(string path);

    // ============================================================
    // Directory operations (async I/O)
    // ============================================================

    Task<Outcome> CreateDirectoryAsync(string path, CancellationToken cancellationToken = default);
    Task<Outcome> DeleteDirectoryAsync(string path, bool recursive = false, CancellationToken cancellationToken = default);

    // ============================================================
    // File enumeration (async I/O)
    // ============================================================

    /// <summary>
    /// Enumerates files matching a single search pattern.
    /// </summary>
    Task<Outcome<IReadOnlyList<string>>> EnumerateFilesAsync(
        string path,
        string searchPattern = "*",
        SearchOption searchOption = SearchOption.TopDirectoryOnly,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Enumerates files matching any of the given search patterns.
    /// Example: EnumerateFilesAsync(path, new[] { "*.log", "*.txt" }, SearchOption.AllDirectories)
    /// </summary>
    Task<Outcome<IReadOnlyList<string>>> EnumerateFilesAsync(
        string path,
        IEnumerable<string> searchPatterns,
        SearchOption searchOption = SearchOption.TopDirectoryOnly,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Convenience method: finds files by extension(s).
    /// Example: FindFilesAsync(path, includeSubdirectories: true, default, ".log", ".txt")
    /// </summary>
    Task<Outcome<IReadOnlyList<string>>> FindFilesAsync(
        string path,
        bool includeSubdirectories,
        CancellationToken cancellationToken = default,
        params string[] extensions);

    /// <summary>
    /// Enumerates subdirectories.
    /// </summary>
    Task<Outcome<IReadOnlyList<string>>> EnumerateDirectoriesAsync(
        string path,
        string searchPattern = "*",
        SearchOption searchOption = SearchOption.TopDirectoryOnly,
        CancellationToken cancellationToken = default);
}