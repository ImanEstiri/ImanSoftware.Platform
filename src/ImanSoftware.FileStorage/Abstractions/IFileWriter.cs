using ImanSoftware.Outcomes;

namespace ImanSoftware.FileStorage;

/// <summary>
/// Contract for writing content to files at any path.
/// No settings needed - paths are used directly.
/// </summary>
public interface IFileWriter
{
    Task<Outcome> WriteAllTextAsync(string path, string content, CancellationToken cancellationToken = default);
    Task<Outcome> WriteAllLinesAsync(string path, IEnumerable<string> lines, CancellationToken cancellationToken = default);
    Task<Outcome> WriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken = default);
    Task<Outcome> AppendAllTextAsync(string path, string content, CancellationToken cancellationToken = default);
}