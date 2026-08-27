using ImanSoftware.Branding.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ImanSoftware.Caching;
using ImanSoftware.Logging;

namespace ImanSoftware.Branding.DependencyInjection;

public static class BrandingServiceCollectionExtensions
{
    public static IServiceCollection AddImanBranding(
        this IServiceCollection services,
        Action<BrandingOptions>? configure = null)
    {
        // 1. تنظیمات
        var options = new BrandingOptions();
        configure?.Invoke(options);
        services.AddSingleton(options);

        services.AddCacheService();
        services.AddLoggerAdapter();

        services.AddHttpClient<IBrandingService, BrandingService>();
        services.TryAddScoped<IBrandingCache, BrandingCache>();

        return services;
    }
}