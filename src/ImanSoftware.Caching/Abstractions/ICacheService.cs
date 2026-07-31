
namespace ImanSoftware.Caching;

/// <summary>
/// Contract for caching data with support for different providers (Memory, Redis, etc.).
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Gets a cached value by key.
    /// </summary>
    T? Get<T>(string key);

    /// <summary>
    /// Sets a value in the cache with an optional expiration time.
    /// </summary>
    void Set<T>(string key, T value, TimeSpan? expiration = null);

    /// <summary>
    /// Removes a value from the cache by key.
    /// </summary>
    void Remove(string key);

    /// <summary>
    /// Checks if a key exists in the cache.
    /// </summary>
    bool Exists(string key);

    /// <summary>
    /// Gets a value or sets it if not exists (atomic operation).
    /// </summary>
    T GetOrSet<T>(string key, Func<T> factory, TimeSpan? expiration = null);
}