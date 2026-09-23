using ImanSoftware.Outcomes;

namespace ImanSoftware.FileStorage;

/// <summary>
/// Contract for directory and path-level operations.
/// </summary>
public interface IDirectoryOperator
{
    // ============================================================
    // Existence checks (sync, no I/O wait)
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
    // Directory operations
    // ============================================================

    /// <summary>
    /// Creates a directory (including all parent directories).
    /// </summary>
    Task<Outcome> CreateDirectoryAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a directory. Set <paramref name="recursive"/> to true to delete contents.
    /// </summary>
    Task<Outcome> DeleteDirectoryAsync(string path, bool recursive = false, CancellationToken cancellationToken = default);

    // ============================================================
    // Enumeration (returns paths only - fast, lazy)
    // ============================================================

    /// <summary>
    /// Enumerates files matching any of the given patterns.
    /// </summary>
    /// <param name="path">Root directory to search.</param>
    /// <param name="searchPatterns">
    /// Wildcard patterns (e.g., "*.log", "app-*.txt"). Pass <c>new[] { "*" }</c> to match all.
    /// </param>
    /// <param name="searchOption">Top directory only or all subdirectories.</param>
    /// <example>
    /// await storage.EnumerateFilesAsync(@"C:\Logs", new[] { "*.log", "*.txt" }, SearchOption.AllDirectories);
    /// </example>
    Task<Outcome<IEnumerable<string>>> EnumerateFilesAsync(
        string path,
        IEnumerable<string> searchPatterns,
        SearchOption searchOption = SearchOption.TopDirectoryOnly,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Enumerates subdirectories.
    /// </summary>
    Task<Outcome<IEnumerable<string>>> EnumerateDirectoriesAsync(
        string path,
        string searchPattern = "*",
        SearchOption searchOption = SearchOption.TopDirectoryOnly,
        CancellationToken cancellationToken = default);

    // ============================================================
    // Search (returns metadata - slower, richer)
    // ============================================================

    /// <summary>
    /// Searches files matching any of the given patterns and returns full metadata for each.
    /// </summary>
    /// <param name="path">Root directory to search.</param>
    /// <param name="searchPatterns">
    /// Wildcard patterns (e.g., "*.log", "app-*.txt"). Pass <c>new[] { "*" }</c> to match all.
    /// </param>
    /// <param name="searchOption">Top directory only or all subdirectories.</param>
    /// <example>
    /// await storage.SearchFilesAsync(@"C:\Logs", new[] { "*.log" }, SearchOption.AllDirectories);
    /// </example>
    Task<Outcome<IEnumerable<FileMetadata>>> SearchFilesAsync(
        string path,
        IEnumerable<string> searchPatterns,
        SearchOption searchOption = SearchOption.TopDirectoryOnly,
        CancellationToken cancellationToken = default);
}