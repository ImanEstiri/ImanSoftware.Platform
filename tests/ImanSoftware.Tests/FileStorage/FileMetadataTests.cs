using ImanSoftware.FileStorage;

namespace ImanSoftware.Tests.FileStorage;

public class FileMetadataTests
{
    private static FileMetadata CreateMetadata(
        long size = 500,
        DateTime? createdAt = null,
        DateTime? lastModifiedAt = null) => new()
    {
        FullPath = Path.Combine(@"C:\data", "report.pdf"),
        FileName = "report.pdf",
        Extension = ".pdf",
        Size = size,
        ContentType = "application/pdf",
        CreatedAt = createdAt ?? new DateTime(2024, 1, 1, 10, 0, 0),
        LastModifiedAt = lastModifiedAt
    };

    [Fact]
    public void SizeInKB_ComputesRoundedKilobytes()
    {
        Assert.Equal(1.25, CreateMetadata(size: 1280).SizeInKB);
    }

    [Fact]
    public void SizeInMB_ComputesRoundedMegabytes()
    {
        Assert.Equal(1.5, CreateMetadata(size: (long)(1.5 * 1024 * 1024)).SizeInMB);
    }

    [Fact]
    public void HumanReadableSize_BytesBelowKB_UsesByteSuffix()
    {
        Assert.EndsWith("500 B", CreateMetadata(size: 500).HumanReadableSize, StringComparison.Ordinal);
    }

    [Fact]
    public void HumanReadableSize_KilobyteRange_UsesKbSuffix()
    {
        Assert.EndsWith("KB", CreateMetadata(size: 1280).HumanReadableSize, StringComparison.Ordinal);
    }

    [Fact]
    public void HumanReadableSize_MegabyteRange_UsesMbSuffix()
    {
        Assert.EndsWith("MB", CreateMetadata(size: 3 * 1024 * 1024).HumanReadableSize, StringComparison.Ordinal);
    }

    [Fact]
    public void WasModified_WhenLastModifiedAfterCreation_ReturnsTrue()
    {
        var metadata = CreateMetadata(
            createdAt: new DateTime(2024, 1, 1),
            lastModifiedAt: new DateTime(2024, 1, 2));

        Assert.True(metadata.WasModified);
    }

    [Fact]
    public void WasModified_WithoutLastModified_ReturnsFalse()
    {
        Assert.False(CreateMetadata(lastModifiedAt: null).WasModified);
    }

    [Fact]
    public void DirectoryPath_ExtractsDirectoryFromFullPath()
    {
        Assert.Equal(@"C:\data", CreateMetadata().DirectoryPath);
    }

    [Fact]
    public void FileNameWithoutExtension_StripsExtension()
    {
        Assert.Equal("report", CreateMetadata().FileNameWithoutExtension);
    }

    [Fact]
    public void ToString_ContainsFileNameAndSize()
    {
        var metadata = CreateMetadata(size: 500);

        Assert.StartsWith("report.pdf (", metadata.ToString(), StringComparison.Ordinal);
        Assert.EndsWith("500 B)", metadata.ToString(), StringComparison.Ordinal);
    }
}
