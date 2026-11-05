using Microsoft.Extensions.DependencyInjection;
using ImanSoftware.FileStorage;
using ImanSoftware.FileStorage.Infrastructure;
using ImanSoftware.Outcomes;

namespace ImanSoftware.Tests.FileStorage;

public class FileStorageRegistrationTests
{
    [Fact]
    public void AddImanFileStorage_RegistersLocalFileStorageAsScoped()
    {
        var services = new ServiceCollection();

        services.AddImanFileStorage();

        var descriptor = Assert.Single(services, d => d.ServiceType == typeof(IFileStorage));
        Assert.Equal(typeof(LocalFileStorage), descriptor.ImplementationType);
        Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [Fact]
    public void AddImanFileStorage_RegistersRoleInterfacesResolvingToSameStorage()
    {
        var services = new ServiceCollection();

        services.AddImanFileStorage();

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var storage = scope.ServiceProvider.GetRequiredService<IFileStorage>();

        Assert.Same(storage, scope.ServiceProvider.GetRequiredService<IFileReader>());
        Assert.Same(storage, scope.ServiceProvider.GetRequiredService<IFileWriter>());
        Assert.Same(storage, scope.ServiceProvider.GetRequiredService<IFileDeleter>());
        Assert.Same(storage, scope.ServiceProvider.GetRequiredService<IFileMover>());
        Assert.Same(storage, scope.ServiceProvider.GetRequiredService<IFileInfoProvider>());
        Assert.Same(storage, scope.ServiceProvider.GetRequiredService<IDirectoryOperator>());
    }

    [Fact]
    public void AddImanFileStorage_WithCustomRegistration_InvokesDelegateInsteadOfDefaults()
    {
        var services = new ServiceCollection();

        services.AddImanFileStorage(s => s.AddSingleton<IFileStorage, StubStorage>());

        Assert.DoesNotContain(services, d => d.ServiceType == typeof(IFileReader));
        using var provider = services.BuildServiceProvider();
        Assert.IsType<StubStorage>(provider.GetRequiredService<IFileStorage>());
    }

    [Fact]
    public void AddImanFileTransfer_WithNullConfigure_ThrowsArgumentNullException()
    {
        var services = new ServiceCollection();

        Assert.Throws<ArgumentNullException>(() => services.AddImanFileTransfer(null!));
    }

    [Fact]
    public void AddImanFileTransfer_RegistersTransferAndAliasesAsScoped()
    {
        var services = new ServiceCollection();

        services.AddImanFileTransfer(o => o.RootPath = "test-root");

        Assert.All(
            new[] { typeof(IFileTransfer), typeof(IFileUploader), typeof(IFileDownloader) },
            type => Assert.Equal(ServiceLifetime.Scoped, services.Single(d => d.ServiceType == type).Lifetime));
    }

    [Fact]
    public void AddImanFileTransfer_BindsOptionsThroughConfiguration()
    {
        var services = new ServiceCollection();

        services.AddImanFileTransfer(o =>
        {
            o.RootPath = "configured-root";
            o.MaxFileSize = 1024;
        });

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<FileTransferOptions>>().Value;

        Assert.Equal("configured-root", options.RootPath);
        Assert.Equal(1024, options.MaxFileSize);
    }

    [Fact]
    public void AddImanFileTransfer_WithCustomRegistration_SkipsDefaults()
    {
        var services = new ServiceCollection();

        services.AddImanFileTransfer(_ => { }, s => s.AddSingleton<IFileTransfer, StubTransfer>());

        Assert.DoesNotContain(services, d => d.ServiceType == typeof(IFileUploader));
        using var provider = services.BuildServiceProvider();
        Assert.IsType<StubTransfer>(provider.GetRequiredService<IFileTransfer>());
    }

    [Fact]
    public void ImanFileStorageFactory_CreateTransfer_AppliesConfigureAndCreatesInstance()
    {
        var transfer = ImanFileStorageFactory.CreateTransfer();

        Assert.IsAssignableFrom<IFileTransfer>(transfer);
    }

    [Fact]
    public void ImanFileStorageFactory_CreateStorage_ReturnsFreshInstances()
    {
        Assert.NotSame(ImanFileStorageFactory.CreateStorage(), ImanFileStorageFactory.CreateStorage());
    }

    private sealed class StubStorage : IFileStorage
    {
        public bool FileExists(string path) => false;
        public bool DirectoryExists(string path) => false;
        public Task<Outcome<string>> ReadAllTextAsync(string path, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<Outcome<string[]>> ReadAllLinesAsync(string path, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<Outcome<byte[]>> ReadAllBytesAsync(string path, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public IAsyncEnumerable<string> ReadLinesAsync(string path, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<Outcome> WriteAllTextAsync(string path, string content, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<Outcome> WriteAllLinesAsync(string path, IEnumerable<string> lines, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<Outcome> WriteAllBytesAsync(string path, byte[] bytes, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<Outcome> AppendAllTextAsync(string path, string content, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<Outcome> DeleteAsync(string filePath) => throw new NotSupportedException();
        public Task<Outcome> MoveAsync(string sourcePath, string destinationPath) => throw new NotSupportedException();
        public Task<Outcome> CopyAsync(string sourcePath, string destinationPath) => throw new NotSupportedException();
        public Task<Outcome<FileMetadata>> GetMetadataAsync(string filePath) => throw new NotSupportedException();
        public Task<Outcome> CreateDirectoryAsync(string path, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<Outcome> DeleteDirectoryAsync(string path, bool recursive = false, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<Outcome<IEnumerable<string>>> EnumerateFilesAsync(string path, IEnumerable<string> searchPatterns, SearchOption searchOption = SearchOption.TopDirectoryOnly, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<Outcome<IEnumerable<string>>> EnumerateDirectoriesAsync(string path, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<Outcome<IEnumerable<FileMetadata>>> SearchFilesAsync(string path, IEnumerable<string> searchPatterns, SearchOption searchOption = SearchOption.TopDirectoryOnly, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }

    private sealed class StubTransfer : IFileTransfer
    {
        public Task<Outcome<string>> UploadAsync(Stream fileStream, string fileName, string? container = null) => throw new NotSupportedException();
        public Task<Outcome<string>> UploadAsync(byte[] fileBytes, string fileName, string? container = null) => throw new NotSupportedException();
        public Task<Outcome<Stream>> DownloadAsync(string filePath) => throw new NotSupportedException();
        public Task<Outcome<byte[]>> DownloadBytesAsync(string filePath) => throw new NotSupportedException();
        public Task<Outcome<string>> GetUrlAsync(string filePath) => throw new NotSupportedException();
    }
}
