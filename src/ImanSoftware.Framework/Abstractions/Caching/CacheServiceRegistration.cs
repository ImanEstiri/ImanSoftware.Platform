using ImanSoftware.Framework.Abstractions.GuidGenerator;
using ImanSoftware.Framework.Infrastructure.Caching;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ImanSoftware.Framework.Abstractions.Caching;

internal static class CacheServiceRegistration
{
    internal static IServiceCollection AddCacheService(
        this IServiceCollection services,
        Type? customImplementation = null)
    {
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