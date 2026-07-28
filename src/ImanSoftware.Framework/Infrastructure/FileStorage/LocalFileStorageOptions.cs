// مسیر: Infrastructure/FileStorage/LocalFileStorageOptions.cs
namespace ImanSoftware.Framework.Infrastructure.FileStorage;

/// <summary>
/// Configuration options for LocalFileStorage.
/// </summary>
public class LocalFileStorageOptions
{
    /// <summary>
    /// Root directory for storing files (e.g., "wwwroot/uploads").
    /// </summary>
    public string RootPath { get; set; } = "wwwroot/uploads";

    /// <summary>
    /// Maximum allowed file size in bytes (default: 100 MB).
    /// </summary>
    public long MaxFileSize { get; set; } = 100 * 1024 * 1024;

    /// <summary>
    /// Allowed file extensions (e.g., ".jpg", ".png", ".pdf"). Empty means all allowed.
    /// </summary>
    public string[] AllowedExtensions { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Whether to create subdirectories based on date (e.g., 2024/01/15/).
    /// </summary>
    public bool UseDateSubdirectories { get; set; } = true;
}