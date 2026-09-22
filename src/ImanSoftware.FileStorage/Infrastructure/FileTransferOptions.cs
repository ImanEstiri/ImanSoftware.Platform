namespace ImanSoftware.FileStorage;

/// <summary>
/// Configuration options for the upload/download storage backend.
/// Only used by <see cref="IFileTransfer"/>.
/// </summary>
public class FileTransferOptions
{
    /// <summary>
    /// Root directory for storing uploaded files.
    /// </summary>
    public string RootPath { get; set; } = "wwwroot/uploads";

    /// <summary>
    /// Maximum allowed file size in bytes (default: 100 MB).
    /// </summary>
    public long MaxFileSize { get; set; } = 100 * 1024 * 1024;

    /// <summary>
    /// Allowed file extensions (e.g., ".jpg", ".png"). Empty means all allowed.
    /// </summary>
    public string[] AllowedExtensions { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Whether to create date-based subdirectories (e.g., 2024/01/15/).
    /// </summary>
    public bool UseDateSubdirectories { get; set; } = true;

    /// <summary>
    /// Base URL prefix for generated URLs (e.g., "/uploads").
    /// </summary>
    public string UrlBasePath { get; set; } = "/uploads";
}