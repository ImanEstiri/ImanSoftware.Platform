using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using ImanSoftware.Caching;
using ImanSoftware.Serialization;

namespace ImanSoftware.Tests.Caching;

public class MemoryCacheServiceTests : IDisposable
{
    private sealed class SampleDto
    {
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
    }

    private readonly MemoryCache _memoryCache = new(Options.Create(new MemoryCacheOptions()));
    private readonly MemoryCacheService _service;

    public MemoryCacheServiceTests()
    {
        _service = new MemoryCacheService(_memoryCache, JsonSerializerFactory.CreateDefaultSystemTextJsonSerializer());
    }

    public void Dispose() => _memoryCache.Dispose();

    [Fact]
    public void Set_ThenGet_RoundTripsValueThroughJson()
    {
        var dto = new SampleDto { Name = "cached", Value = 9 };

        _service.Set("key", dto);

        var result = _service.Get<SampleDto>("key");
        Assert.NotNull(result);
        Assert.Equal("cached", result!.Name);
        Assert.Equal(9, result.Value);
    }

    [Fact]
    public void Get_MissingKey_ReturnsDefault()
    {
        Assert.Null(_service.Get<SampleDto>("missing"));
        Assert.Equal(0, _service.Get<int>("missing"));
    }

    [Fact]
    public void Set_StoresValueAsSerializedString()
    {
        _service.Set("key", new SampleDto { Name = "stored", Value = 1 });

        Assert.True(_memoryCache.TryGetValue("key", out string? json));
        Assert.Contains("\"name\":\"stored\"", json, StringComparison.Ordinal);
    }

    [Fact]
    public void Get_AfterExpiration_ReturnsDefault()
    {
        _service.Set("key", "value", TimeSpan.FromMilliseconds(50));

        Thread.Sleep(150);

        Assert.Null(_service.Get<string>("key"));
    }

    [Fact]
    public void Exists_AfterSet_ReturnsTrue()
    {
        _service.Set("key", "value");

        Assert.True(_service.Exists("key"));
    }

    [Fact]
    public void Exists_ForMissingKey_ReturnsFalse()
    {
        Assert.False(_service.Exists("missing"));
    }

    [Fact]
    public void Remove_DeletesEntry()
    {
        _service.Set("key", "value");

        _service.Remove("key");

        Assert.False(_service.Exists("key"));
    }

    [Fact]
    public void Remove_MissingKey_DoesNotThrow()
    {
        _service.Remove("missing");
    }

    [Fact]
    public void GetOrSet_OnMiss_InvokesFactoryAndCaches()
    {
        var calls = 0;

        var value = _service.GetOrSet("key", () => { calls++; return new SampleDto { Name = "factory", Value = 1 }; });

        Assert.Equal(1, calls);
        Assert.Equal("factory", value!.Name);
        Assert.True(_service.Exists("key"));
    }

    [Fact]
    public void GetOrSet_OnHit_DoesNotInvokeFactoryAgain()
    {
        var calls = 0;

        _service.GetOrSet("key", () => { calls++; return new SampleDto { Name = "factory", Value = 1 }; });
        _service.GetOrSet("key", () => { calls++; return new SampleDto { Name = "other", Value = 2 }; });

        Assert.Equal(1, calls);
        Assert.Equal("factory", _service.Get<SampleDto>("key")!.Name);
    }

    [Fact]
    public void GetOrSet_WithExpiration_AppliesExpiration()
    {
        _service.GetOrSet("key", () => "value", TimeSpan.FromMilliseconds(50));

        Thread.Sleep(150);

        Assert.Null(_service.Get<string>("key"));
    }

    [Fact]
    public void Set_WithNullValue_EntryExistsButGetReturnsDefault()
    {
        _service.Set<SampleDto?>("key", null);

        Assert.True(_service.Exists("key"));

        Assert.Null(_service.Get<SampleDto>("key"));
    }
}
