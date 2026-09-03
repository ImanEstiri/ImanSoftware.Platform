using ImanSoftware.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ImanSoftware.Caching;

public static class CacheServiceRegistration
{
    public static IServiceCollection AddCacheService(
        this IServiceCollection services,
        Type? customImplementation = null)
    {
        services.AddImanSerialization(ImplementationType: SerializationDefaultImplementationType.SystemText);
        services.AddMemoryCache();
        if (customImplementation is not null)
        {
            services.AddSingleton(typeof(ICacheService), customImplementation);
        }
        else 
        {
            services.TryAddSingleton<ICacheService, MemoryCacheService>();
        }

        return services;
    }
}