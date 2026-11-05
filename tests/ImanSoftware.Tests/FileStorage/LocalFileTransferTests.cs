using ImanSoftware.FileStorage;
using ImanSoftware.FileStorage.Infrastructure;
using ImanSoftware.Outcomes;

namespace ImanSoftware.Tests.FileStorage;

public sealed class LocalFileTransferTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "iman-filetransfer-" + Guid.NewGuid().ToString("N"));

    public void Dispose()
    {
        if (Directory.Exists(_root))
        {
            Directory.Delete(_root, recursive: true);
        }
    }

    private IFileTransfer CreateTransfer(Action<FileTransferOptions>? configure = null) =>
        ImanFileStorageFactory.CreateTransfer(options =>
        {
            options.RootPath = _root;
            options.UseDateSubdirectories = false;
            configure?.Invoke(options);
        });

    [Fact]
    public void Ctor_WithNullOptions_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new LocalFileTransfer(null!));
    }

    [Fact]
    public void Ctor_CreatesRootDirectoryWhenMissing()
    {
        var nestedRoot = Path.Combine(_root, "created-by-ctor");
        Assert.False(Directory.Exists(nestedRoot));

        ImanFileStorageFactory.CreateTransfer(new FileTransferOptions { RootPath = nestedRoot });

        Assert.True(Directory.Exists(nestedRoot));
    }

    [Fact]
    public async Task UploadAsync_WritesFileAndReturnsRelativePath()
    {
        var transfer = CreateTransfer();

        var result = await transfer.UploadAsync("content"u8.ToArray(), "notes.txt");

        Assert.True(result.IsSuccess);
        var uploadedPath = Path.Combine(_root, result.Data);
        Assert.True(File.Exists(uploadedPath));
        Assert.Equal("content", await File.ReadAllTextAsync(uploadedPath));
        Assert.EndsWith(".txt", result.Data, StringComparison.Ordinal);
    }

    [Fact]
    public async Task UploadAsync_GeneratesUniqueNamesForSameFileName()
    {
        var transfer = CreateTransfer();

        var first = await transfer.UploadAsync("a"u8.ToArray(), "file.txt");
        var second = await transfer.UploadAsync("b"u8.ToArray(), "file.txt");

        Assert.NotEqual(first.Data, second.Data);
    }

    [Fact]
    public async Task UploadAsync_WithDateSubdirectories_UsesYearMonthDayPath()
    {
        var transfer = CreateTransfer(options => options.UseDateSubdirectories = true);

        var result = await transfer.UploadAsync("a"u8.ToArray(), "file.txt");

        var today = DateTime.Now;
        var expectedPrefix = Path.Combine(
            today.ToString("yyyy"),
            today.ToString("MM"),
            today.ToString("dd"));
        Assert.StartsWith(expectedPrefix, result.Data, StringComparison.Ordinal);
    }

    [Fact]
    public async Task UploadAsync_WithContainer_PlacesFileInsideContainer()
    {
        var transfer = CreateTransfer();

        var result = await transfer.UploadAsync("a"u8.ToArray(), "file.txt", container: "avatars");

        Assert.StartsWith("avatars", result.Data, StringComparison.Ordinal);
        Assert.True(File.Exists(Path.Combine(_root, result.Data)));
    }

    [Fact]
    public async Task UploadAsync_OversizedFile_ReturnsValidationFailure()
    {
        var transfer = CreateTransfer(options => options.MaxFileSize = 4);

        var result = await transfer.UploadAsync(new byte[10], "big.txt");

        Assert.True(result.IsFailure);
        Assert.Equal("FILE_TOO_LARGE", result.Error.Code);
        Assert.Equal("File size exceeds maximum allowed (0 MB).", result.Error.Message);
        Assert.Equal(OutcomeErrorType.Validation, result.Error.Type);
    }

    [Fact]
    public async Task UploadAsync_DisallowedExtension_ReturnsValidationFailure()
    {
        var transfer = CreateTransfer(options => options.AllowedExtensions = [".jpg", ".png"]);

        var result = await transfer.UploadAsync("data"u8.ToArray(), "script.exe");

        Assert.True(result.IsFailure);
        Assert.Equal("FILE_EXTENSION_NOT_ALLOWED", result.Error.Code);
        Assert.Equal("File extension '.exe' is not allowed.", result.Error.Message);
    }

    [Fact]
    public async Task UploadAsync_AllowedExtension_Succeeds()
    {
        var transfer = CreateTransfer(options => options.AllowedExtensions = [".jpg", ".PNG"]);

        var result = await transfer.UploadAsync("data"u8.ToArray(), "photo.png");

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task DownloadAsync_ReturnsFileStreamForExistingFile()
    {
        var transfer = CreateTransfer();
        await File.WriteAllTextAsync(Path.Combine(_root, "stored.txt"), "downloaded");

        var result = await transfer.DownloadAsync("stored.txt");

        Assert.True(result.IsSuccess);
        using var reader = new StreamReader(result.Data);
        Assert.Equal("downloaded", await reader.ReadToEndAsync());
    }

    [Fact]
    public async Task DownloadAsync_MissingFile_ReturnsFileNotFound()
    {
        var result = await CreateTransfer().DownloadAsync("missing.txt");

        Assert.True(result.IsFailure);
        Assert.Equal("FILE_NOT_FOUND", result.Error.Code);
        Assert.Equal(OutcomeErrorType.NotFound, result.Error.Type);
    }

    [Fact]
    public async Task DownloadBytesAsync_ReturnsFileBytes()
    {
        var transfer = CreateTransfer();
        await File.WriteAllTextAsync(Path.Combine(_root, "bytes.txt"), "payload");

        var result = await transfer.DownloadBytesAsync("bytes.txt");

        Assert.True(result.IsSuccess);
        Assert.Equal("payload", System.Text.Encoding.UTF8.GetString(result.Data));
    }

    [Fact]
    public async Task DownloadAsync_WithPathTraversal_ReturnsFailure()
    {
        var transfer = CreateTransfer();
        var traversalPath = Path.Combine("..", "outside.txt");

        var result = await transfer.DownloadAsync(traversalPath);

        // The SecurityException from GetFullPath is converted into a failed Outcome.
        Assert.True(result.IsFailure);
        Assert.Contains("Invalid path traversal attempt.", result.Error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task GetUrlAsync_BuildsUrlWithBasePathAndForwardSlashes()
    {
        var transfer = CreateTransfer(options => options.UrlBasePath = "/static-uploads");

        var result = await transfer.GetUrlAsync(@"2024/05/file.txt");

        Assert.True(result.IsSuccess);
        Assert.Equal("/static-uploads/2024/05/file.txt", result.Data);
    }

    [Fact]
    public async Task GetUrlAsync_TrimsTrailingSlashFromBasePath()
    {
        var transfer = CreateTransfer(options => options.UrlBasePath = "/uploads/");

        var result = await transfer.GetUrlAsync("a.txt");

        Assert.Equal("/uploads/a.txt", result.Data);
    }
}
