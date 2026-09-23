namespace ImanSoftware.FileStorage;

/// <summary>
/// Represents metadata information of a file.
/// Immutable value object returned by search and metadata operations.
/// </summary>
public sealed record FileMetadata
{
    /// <summary>
    /// Full absolute path to the file (e.g., "C:\Logs\2024\01\app.log").
    /// </summary>
    public string FullPath { get; init; } = string.Empty;

    /// <summary>
    /// File name including extension (e.g., "app.log").
    /// </summary>
    public string FileName { get; init; } = string.Empty;

    /// <summary>
    /// File extension in lowercase, including the dot (e.g., ".log").
    /// </summary>
    public string Extension { get; init; } = string.Empty;

    /// <summary>
    /// File size in bytes.
    /// </summary>
    public long Size { get; init; }

    /// <summary>
    /// MIME content type (e.g., "text/plain", "image/png").
    /// </summary>
    public string ContentType { get; init; } = string.Empty;

    /// <summary>
    /// File creation time (local time).
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Last modification time (local time). Null if not available.
    /// </summary>
    public DateTime? LastModifiedAt { get; init; }

    // ============================================================
    // Convenience properties
    // ============================================================

    /// <summary>
    /// File size in kilobytes (rounded to 2 decimals).
    /// </summary>
    public double SizeInKB => Math.Round(Size / 1024.0, 2);

    /// <summary>
    /// File size in megabytes (rounded to 2 decimals).
    /// </summary>
    public double SizeInMB => Math.Round(Size / 1024.0 / 1024.0, 2);

    /// <summary>
    /// File size in gigabytes (rounded to 2 decimals).
    /// </summary>
    public double SizeInGB => Math.Round(Size / 1024.0 / 1024.0 / 1024.0, 2);

    /// <summary>
    /// Human-readable file size (e.g., "1.25 MB", "512 B").
    /// </summary>
    public string HumanReadableSize => Size switch
    {
        < 1024 => $"{Size} B",
        < 1024 * 1024 => $"{SizeInKB:F2} KB",
        < 1024 * 1024 * 1024 => $"{SizeInMB:F2} MB",
        _ => $"{SizeInGB:F2} GB"
    };

    /// <summary>
    /// Whether the file has been modified since its creation.
    /// </summary>
    public bool WasModified => LastModifiedAt.HasValue && LastModifiedAt.Value > CreatedAt;

    /// <summary>
    /// Directory path containing the file.
    /// </summary>
    public string DirectoryPath => Path.GetDirectoryName(FullPath) ?? string.Empty;

    /// <summary>
    /// File name without extension (e.g., "app" for "app.log").
    /// </summary>
    public string FileNameWithoutExtension => Path.GetFileNameWithoutExtension(FileName);

    // ============================================================
    // Display
    // ============================================================

    /// <summary>
    /// Returns a friendly string representation.
    /// </summary>
    public override string ToString()
        => $"{FileName} ({HumanReadableSize})";
}