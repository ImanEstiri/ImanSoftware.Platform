// مسیر: Abstractions/RateLimiterRegistration.cs
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
        var options = new RateLimiterOptions();
        configureOptions(options);
        services.AddSingleton(options);

        if (customRegistration is not null)
        {
            customRegistration(services);
        }
        else
        {
            services.TryAddSingleton<IRateLimiterStrategy>(provider =>
            {
                return options.DefaultStrategy switch
                {
                    RateLimiterStrategyType.FixedWindow => new FixedWindowStrategy(),
                    //RateLimiterStrategyType.SlidingWindow => new SlidingWindowStrategy(),
                    //RateLimiterStrategyType.TokenBucket => new TokenBucketStrategy(),
                    _ => new FixedWindowStrategy()
                };
            });

            services.TryAddSingleton<IRateLimiter, InMemoryRateLimiter>();
        }

        return services;
    }
}