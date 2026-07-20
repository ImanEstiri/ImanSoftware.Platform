using ImanSoftware.Framework.Abstractions.Caching;
using ImanSoftware.Framework.Abstractions.Serialization;
using Microsoft.Extensions.Caching.Memory;

namespace ImanSoftware.Framework.Infrastructure.Caching;

/// <summary>
/// Implementation of ICacheService using IMemoryCache (in-memory caching).
/// Uses IJsonSerializer to store objects as JSON strings.
/// </summary>
internal class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IJsonSerializer _jsonSerializer;

    public MemoryCacheService(IMemoryCache memoryCache, IJsonSerializer jsonSerializer)
    {
        _memoryCache = memoryCache;
        _jsonSerializer = jsonSerializer;
    }

    public T? Get<T>(string key)
    {
        var json = _memoryCache.Get<string>(key);
        if (json is null)
            return default;

        return _jsonSerializer.Deserialize<T>(json);
    }

    public void Set<T>(string key, T value, TimeSpan? expiration = null)
    {
        var json = _jsonSerializer.Serialize(value);
        var options = new MemoryCacheEntryOptions();

        if (expiration.HasValue)
            options.AbsoluteExpirationRelativeToNow = expiration.Value;

        _memoryCache.Set(key, json, options);
    }

    public void Remove(string key)
    {
        _memoryCache.Remove(key);
    }

    public bool Exists(string key)
    {
        return _memoryCache.TryGetValue(key, out _);
    }

    public T GetOrSet<T>(string key, Func<T> factory, TimeSpan? expiration = null)
    {
        // استفاده از GetOrCreate برای جلوگیری از Race Condition
        var json = _memoryCache.GetOrCreate<string>(key, entry =>
        {
            if (expiration.HasValue)
                entry.AbsoluteExpirationRelativeToNow = expiration.Value;

            var value = factory();
            return _jsonSerializer.Serialize(value);
        });

        // اگر json null برگردد (که نباید اینطور باشد)، مقدار را دوباره تولید می‌کنیم
        if (json is null)
            return factory();

        return _jsonSerializer.Deserialize<T>(json);
    }
}