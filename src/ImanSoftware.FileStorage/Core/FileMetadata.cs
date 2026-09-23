namespace ImanSoftware.FileStorage;

/// <summary>
/// Represents metadata information of a stored file.
/// </summary>
public class FileMetadata
{
    public string FileName { get; set; } = string.Empty;
    public long Size { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public Dictionary<string, object>? CustomProperties { get; set; }
}