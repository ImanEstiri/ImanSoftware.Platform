using ImanSoftware.FileStorage;
using ImanSoftware.FileStorage.Infrastructure;
using ImanSoftware.Outcomes;

namespace ImanSoftware.Tests.FileStorage;

public sealed class LocalFileStorageTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "iman-filestorage-" + Guid.NewGuid().ToString("N"));
    private readonly LocalFileStorage _storage = ImanFileStorageFactory.CreateStorage() as LocalFileStorage
        ?? throw new InvalidOperationException("Factory must produce LocalFileStorage.");

    public LocalFileStorageTests() => Directory.CreateDirectory(_root);

    public void Dispose()
    {
        if (Directory.Exists(_root))
        {
            Directory.Delete(_root, recursive: true);
        }
    }

    private string PathOf(string fileName) => Path.Combine(_root, fileName);

    // ---------- Write ----------

    [Fact]
    public async Task WriteAllTextAsync_CreatesFileAndParentDirectories()
    {
        var path = PathOf("nested/dir/file.txt");

        var result = await _storage.WriteAllTextAsync(path, "hello");

        Assert.True(result.IsSuccess);
        Assert.Equal("hello", await File.ReadAllTextAsync(path));
    }

    [Fact]
    public async Task WriteAllBytesAsync_WritesBinaryContent()
    {
        var path = PathOf("bin/data.bin");
        var bytes = new byte[] { 1, 2, 3, 250 };

        var result = await _storage.WriteAllBytesAsync(path, bytes);

        Assert.True(result.IsSuccess);
        Assert.Equal(bytes, await File.ReadAllBytesAsync(path));
    }

    [Fact]
    public async Task WriteAllLinesAsync_WritesLineSeparatedContent()
    {
        var path = PathOf("lines.txt");

        var result = await _storage.WriteAllLinesAsync(path, ["a", "b", "c"]);

        Assert.True(result.IsSuccess);
        Assert.Equal(3, (await File.ReadAllLinesAsync(path)).Length);
    }

    [Fact]
    public async Task AppendAllTextAsync_AddsToEndOfFile()
    {
        var path = PathOf("append.txt");
        await _storage.WriteAllTextAsync(path, "start");

        var result = await _storage.AppendAllTextAsync(path, "-end");

        Assert.True(result.IsSuccess);
        Assert.Equal("start-end", await File.ReadAllTextAsync(path));
    }

    // ---------- Read ----------

    [Fact]
    public async Task ReadAllTextAsync_ReturnsContent()
    {
        var path = PathOf("read.txt");
        await File.WriteAllTextAsync(path, "content");

        var result = await _storage.ReadAllTextAsync(path);

        Assert.True(result.IsSuccess);
        Assert.Equal("content", result.Data);
    }

    [Fact]
    public async Task ReadAllTextAsync_MissingFile_ReturnsFileNotFoundOutcome()
    {
        var result = await _storage.ReadAllTextAsync(PathOf("missing.txt"));

        Assert.True(result.IsFailure);
        Assert.Equal("FILE_NOT_FOUND", result.Error.Code);
        Assert.Equal("File not found.", result.Error.Message);
        Assert.Equal(OutcomeErrorType.NotFound, result.Error.Type);
    }

    [Fact]
    public async Task ReadAllLinesAsync_ReturnsLines()
    {
        var path = PathOf("lines.txt");
        await File.WriteAllLinesAsync(path, ["one", "two"]);

        var result = await _storage.ReadAllLinesAsync(path);

        Assert.True(result.IsSuccess);
        Assert.Equal(new[] { "one", "two" }, result.Data);
    }

    [Fact]
    public async Task ReadAllBytesAsync_ReturnsBytes()
    {
        var path = PathOf("data.bin");
        var bytes = new byte[] { 9, 8, 7 };
        await File.WriteAllBytesAsync(path, bytes);

        var result = await _storage.ReadAllBytesAsync(path);

        Assert.True(result.IsSuccess);
        Assert.Equal(bytes, result.Data);
    }

    [Fact]
    public async Task ReadLinesAsync_StreamsLinesLazily()
    {
        var path = PathOf("stream.txt");
        await File.WriteAllLinesAsync(path, ["l1", "l2", "l3"]);

        var lines = new List<string>();
        await foreach (var line in _storage.ReadLinesAsync(path))
        {
            lines.Add(line);
        }

        Assert.Equal(new[] { "l1", "l2", "l3" }, lines);
    }

    // ---------- Delete / Move / Copy ----------

    [Fact]
    public async Task DeleteAsync_RemovesFile()
    {
        var path = PathOf("delete-me.txt");
        await File.WriteAllTextAsync(path, "x");

        var result = await _storage.DeleteAsync(path);

        Assert.True(result.IsSuccess);
        Assert.False(File.Exists(path));
    }

    [Fact]
    public async Task DeleteAsync_MissingFile_ReturnsFileNotFound()
    {
        var result = await _storage.DeleteAsync(PathOf("missing.txt"));

        Assert.True(result.IsFailure);
        Assert.Equal("FILE_NOT_FOUND", result.Error.Code);
    }

    [Fact]
    public async Task MoveAsync_RetargetsFile()
    {
        var source = PathOf("source.txt");
        var destination = PathOf("moved/destination.txt");
        await File.WriteAllTextAsync(source, "payload");

        var result = await _storage.MoveAsync(source, destination);

        Assert.True(result.IsSuccess);
        Assert.False(File.Exists(source));
        Assert.Equal("payload", await File.ReadAllTextAsync(destination));
    }

    [Fact]
    public async Task MoveAsync_MissingSource_ReturnsSourceNotFound()
    {
        var result = await _storage.MoveAsync(PathOf("missing.txt"), PathOf("target.txt"));

        Assert.True(result.IsFailure);
        Assert.Equal("FILE_NOT_FOUND", result.Error.Code);
        Assert.Equal("Source file not found.", result.Error.Message);
    }

    [Fact]
    public async Task CopyAsync_DuplicatesFileContent()
    {
        var source = PathOf("original.txt");
        var destination = PathOf("copied/copy.txt");
        await File.WriteAllTextAsync(source, "data");

        var result = await _storage.CopyAsync(source, destination);

        Assert.True(result.IsSuccess);
        Assert.True(File.Exists(source));
        Assert.Equal("data", await File.ReadAllTextAsync(destination));
    }

    [Fact]
    public async Task CopyAsync_MissingSource_ReturnsSourceNotFound()
    {
        var result = await _storage.CopyAsync(PathOf("missing.txt"), PathOf("target.txt"));

        Assert.True(result.IsFailure);
        Assert.Equal("FILE_NOT_FOUND", result.Error.Code);
        Assert.Equal(OutcomeErrorType.NotFound, result.Error.Type);
    }

    // ---------- Metadata ----------

    [Fact]
    public async Task GetMetadataAsync_ReturnsFileDetails()
    {
        var path = PathOf("meta.json");
        await File.WriteAllTextAsync(path, "{}");

        var result = await _storage.GetMetadataAsync(path);

        Assert.True(result.IsSuccess);
        Assert.Equal("meta.json", result.Data.FileName);
        Assert.Equal(".json", result.Data.Extension);
        Assert.Equal("application/json", result.Data.ContentType);
        Assert.Equal(2, result.Data.Size);
    }

    [Theory]
    [InlineData(".txt", "text/plain")]
    [InlineData(".png", "image/png")]
    [InlineData(".pdf", "application/pdf")]
    [InlineData(".mp4", "video/mp4")]
    [InlineData(".woff2", "font/woff2")]
    [InlineData(".unknownext", "application/octet-stream")]
    public async Task GetMetadataAsync_MapsKnownExtensionsToContentTypes(string extension, string expected)
    {
        var path = PathOf("file" + extension);
        await File.WriteAllTextAsync(path, "x");

        var result = await _storage.GetMetadataAsync(path);

        Assert.True(result.IsSuccess);
        Assert.Equal(expected, result.Data.ContentType);
    }

    [Fact]
    public async Task GetMetadataAsync_MissingFile_ReturnsFileNotFound()
    {
        var result = await _storage.GetMetadataAsync(PathOf("missing.txt"));

        Assert.True(result.IsFailure);
        Assert.Equal("FILE_NOT_FOUND", result.Error.Code);
    }

    // ---------- Directories ----------

    [Fact]
    public async Task CreateDirectoryAsync_CreatesNestedDirectories()
    {
        var result = await _storage.CreateDirectoryAsync(PathOf("a/b/c"));

        Assert.True(result.IsSuccess);
        Assert.True(Directory.Exists(PathOf("a/b/c")));
    }

    [Fact]
    public async Task DeleteDirectoryAsync_RemovesDirectoryRecursively()
    {
        var dir = PathOf("todelete");
        Directory.CreateDirectory(Path.Combine(dir, "child"));
        await File.WriteAllTextAsync(Path.Combine(dir, "child", "f.txt"), "x");

        var result = await _storage.DeleteDirectoryAsync(dir, recursive: true);

        Assert.True(result.IsSuccess);
        Assert.False(Directory.Exists(dir));
    }

    [Fact]
    public async Task DeleteDirectoryAsync_MissingDirectory_ReturnsDirNotFound()
    {
        var result = await _storage.DeleteDirectoryAsync(PathOf("missing-dir"));

        Assert.True(result.IsFailure);
        Assert.Equal("DIR_NOT_FOUND", result.Error.Code);
        Assert.Equal("Directory not found.", result.Error.Message);
    }

    [Fact]
    public async Task FileExists_And_DirectoryExists_ReportAccurately()
    {
        var file = PathOf("exists.txt");
        await File.WriteAllTextAsync(file, "x");

        Assert.True(_storage.FileExists(file));
        Assert.False(_storage.FileExists(PathOf("nope.txt")));
        Assert.True(_storage.DirectoryExists(_root));
        Assert.False(_storage.DirectoryExists(PathOf("nope-dir")));
    }

    [Fact]
    public async Task EnumerateFilesAsync_WithMultiplePatterns_FiltersByFileNameCaseInsensitively()
    {
        var dir = PathOf("enums");
        Directory.CreateDirectory(dir);
        await File.WriteAllTextAsync(Path.Combine(dir, "a.log"), "x");
        await File.WriteAllTextAsync(Path.Combine(dir, "b.TXT"), "x");
        await File.WriteAllTextAsync(Path.Combine(dir, "c.json"), "x");

        var result = await _storage.EnumerateFilesAsync(dir, ["*.log", " *.TXT "]);

        Assert.True(result.IsSuccess);
        var names = result.Data.Select(Path.GetFileName).OrderBy(n => n).ToArray();
        Assert.Equal(new[] { "a.log", "b.TXT" }, names);
    }

    [Fact]
    public async Task EnumerateFilesAsync_MissingDirectory_ReturnsDirNotFound()
    {
        var result = await _storage.EnumerateFilesAsync(PathOf("missing-dir"), ["*"]);

        Assert.True(result.IsFailure);
        Assert.Equal("DIR_NOT_FOUND", result.Error.Code);
    }

    [Fact]
    public async Task EnumerateDirectoriesAsync_ReturnsMatchingSubdirectories()
    {
        var dir = PathOf("dirs");
        Directory.CreateDirectory(Path.Combine(dir, "logs-1"));
        Directory.CreateDirectory(Path.Combine(dir, "logs-2"));
        Directory.CreateDirectory(Path.Combine(dir, "other"));

        var result = await _storage.EnumerateDirectoriesAsync(dir, "logs-*");

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Data.Count());
    }

    [Fact]
    public async Task SearchFilesAsync_ReturnsMetadataForMatches()
    {
        var dir = PathOf("search");
        Directory.CreateDirectory(dir);
        await File.WriteAllTextAsync(Path.Combine(dir, "notes.txt"), "abc");

        var result = await _storage.SearchFilesAsync(dir, ["*.txt"]);

        Assert.True(result.IsSuccess);
        var metadata = Assert.Single(result.Data);
        Assert.Equal("notes.txt", metadata.FileName);
        Assert.Equal(3, metadata.Size);
    }

    [Fact]
    public async Task SearchFilesAsync_MissingDirectory_ReturnsDirNotFound()
    {
        var result = await _storage.SearchFilesAsync(PathOf("missing-dir"), ["*"]);

        Assert.True(result.IsFailure);
        Assert.Equal("DIR_NOT_FOUND", result.Error.Code);
    }
}
