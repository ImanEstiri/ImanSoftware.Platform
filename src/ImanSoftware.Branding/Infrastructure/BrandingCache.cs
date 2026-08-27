
using ImanSoftware.Caching; 

namespace ImanSoftware.Branding.Infrastructure;

internal class BrandingCache : IBrandingCache
{
    private readonly ICacheService _cache;
    private readonly BrandingOptions _options;

    public BrandingCache(ICacheService cache, BrandingOptions options)
    {
        _cache = cache;
        _options = options;
    }

    public async Task<BadgeData?> GetAsync(string key)
    {
        return await Task.FromResult(_cache.Get<BadgeData>(key));
    }

    public async Task SetAsync(string key, BadgeData value)
    {
        await Task.Run(() => _cache.Set(key, value, _options.CacheDurationHours));
    }
}