using ImanSoftware.RateLimiter.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ImanSoftware.RateLimiter;

internal static class RateLimiterRegistration
{
    public static IServiceCollection AddRateLimiter(
        this IServiceCollection services,
        Action<RateLimiterOptions> configureOptions,
        Action<IServiceCollection>? customRegistration = null)
    {
        if (configureOptions is null)
            throw new ArgumentNullException(nameof(configureOptions));

        var options = new RateLimiterOptions();
        configureOptions(options);

        services.AddSingleton(options);

        if (customRegistration is not null)
        {
            customRegistration(services);
        }
        else
        {
            services.TryAddSingleton<IRateLimiterStrategy, FixedWindowStrategy>();
            services.TryAddSingleton<IRateLimiter, InMemoryRateLimiter>();
        }

        return services;
    }
}