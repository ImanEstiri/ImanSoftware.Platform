using ImanSoftware.Extensions;

namespace ImanSoftware.Tests.Extensions;

public class EnumerableExtensionsTests
{
    private sealed record Item(int Id, string Name);

    [Fact]
    public void IsNullOrEmpty_WithNull_ReturnsTrue()
    {
        IEnumerable<int>? source = null;

        Assert.True(source.IsNullOrEmpty());
    }

    [Fact]
    public void IsNullOrEmpty_WithEmpty_ReturnsTrue()
    {
        Assert.True(Array.Empty<int>().IsNullOrEmpty());
    }

    [Fact]
    public void IsNullOrEmpty_WithItems_ReturnsFalse()
    {
        Assert.False(new[] { 1 }.IsNullOrEmpty());
    }

    [Fact]
    public void ToHashSet_RemovesDuplicates()
    {
        var set = EnumerableExtensions.ToHashSet(new[] { 1, 2, 2, 3 });

        Assert.Equal(new HashSet<int> { 1, 2, 3 }, set);
    }

    [Fact]
    public void ForEach_ExecutesActionForEveryItem()
    {
        var collected = new List<int>();

        new[] { 1, 2, 3 }.ForEach(collected.Add);

        Assert.Equal(new[] { 1, 2, 3 }, collected);
    }

    [Fact]
    public void ForEach_WithNullAction_ThrowsNullReferenceException()
    {
        Assert.Throws<NullReferenceException>(() => new[] { 1 }.ForEach(null!));
    }

    [Fact]
    public void DistinctBy_KeepsFirstOccurrencePerKey()
    {
        var items = new[]
        {
            new Item(1, "first"),
            new Item(2, "second"),
            new Item(1, "third")
        };

        var result = EnumerableExtensions.DistinctBy(items, i => i.Id).ToArray();

        Assert.Equal(2, result.Length);
        Assert.Equal("first", result[0].Name);
        Assert.Equal("second", result[1].Name);
    }

    [Fact]
    public void DistinctBy_IsDeferred()
    {
        var enumerated = false;
        IEnumerable<Item> Source()
        {
            enumerated = true;
            yield return new Item(1, "only");
        }

        _ = EnumerableExtensions.DistinctBy(Source(), i => i.Id);

        Assert.False(enumerated);
    }

    [Fact]
    public void Chunk_SplitsIntoEqualChunksPlusRemainder()
    {
        var chunks = EnumerableExtensions.Chunk(Enumerable.Range(1, 5), 2).Select(c => c.ToArray()).ToArray();

        Assert.Equal(3, chunks.Length);
        Assert.Equal(new[] { 1, 2 }, chunks[0]);
        Assert.Equal(new[] { 3, 4 }, chunks[1]);
        Assert.Equal(new[] { 5 }, chunks[2]);
    }

    [Fact]
    public void Chunk_EmptySource_YieldsNoChunks()
    {
        Assert.Empty(EnumerableExtensions.Chunk(Array.Empty<int>(), 3));
    }

    [Fact]
    public void Chunk_ChunkSizeLargerThanSource_YieldsSinglePartialChunk()
    {
        var chunks = EnumerableExtensions.Chunk(new[] { 1, 2 }, 10).Select(c => c.ToArray()).ToArray();

        var chunk = Assert.Single(chunks);
        Assert.Equal(new[] { 1, 2 }, chunk);
    }

    [Fact]
    public void Chunk_InvalidChunkSize_ThrowsOnEnumerationNotOnCall()
    {
        var query = EnumerableExtensions.Chunk(Enumerable.Range(1, 3), 0);

        var exception = Assert.Throws<ArgumentException>(() => query.ToList());

        Assert.Equal("Chunk size must be positive. (Parameter 'chunkSize')", exception.Message);
    }

    [Fact]
    public void Chunk_EachChunkIsIndependent()
    {
        var chunks = EnumerableExtensions.Chunk(Enumerable.Range(1, 4), 2).ToList();

        // Mutating one chunk must not affect the others.
        ((List<int>)chunks[0]).Add(99);

        Assert.Equal(new[] { 3, 4 }, chunks[1]);
    }
}
