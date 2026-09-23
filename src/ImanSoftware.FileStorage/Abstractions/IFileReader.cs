using ImanSoftware.Outcomes;

namespace ImanSoftware.FileStorage;

/// <summary>
/// Contract for reading file content from any path.
/// </summary>
public interface IFileReader
{
    Task<Outcome<string>> ReadAllTextAsync(string path, CancellationToken cancellationToken = default);
    Task<Outcome<string[]>> ReadAllLinesAsync(string path, CancellationToken cancellationToken = default);
    Task<Outcome<byte[]>> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Streams lines lazily (memory-efficient for large files).
    /// </summary>
    IAsyncEnumerable<string> ReadLinesAsync(string path, CancellationToken cancellationToken = default);
}