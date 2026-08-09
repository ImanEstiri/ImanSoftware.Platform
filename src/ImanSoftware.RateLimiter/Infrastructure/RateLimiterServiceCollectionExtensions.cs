using ImanSoftware.RateLimiter.Abstractions;
using ImanSoftware.RateLimiter.Core.Enums;
using ImanSoftware.RateLimiter.Core.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.RateLimiting;

namespace ImanSoftware.RateLimiter.Infrastructure;

/// <summary>
/// Extension methods for registering the ImanSoftware Rate Limiter.
/// </summary>
public static class RateLimiterServiceCollectionExtensions
{
    /// <summary>
    /// Adds the ImanSoftware Rate Limiter with default configuration.
    /// </summary>
    public static IServiceCollection AddImanRateLimiter(this IServiceCollection services)
        => services.AddImanRateLimiter(_ => { });

    /// <summary>
    /// Adds the ImanSoftware Rate Limiter with custom configuration.
    /// </summary>
    public static IServiceCollection AddImanRateLimiter(
        this IServiceCollection services,
        Action<ImanRateLimiterOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        var options = new ImanRateLimiterOptions();
        configure(options);

        services.AddSingleton(options);

        if (options.EnableMetrics)
            services.AddSingleton<IRateLimiterMetrics, RateLimiterMetrics>();

        services.AddSingleton<IRateLimiterPolicy, DefaultRateLimiterPolicy>();

        services.AddRateLimiter(rateLimiterOptions =>
        {
            // Global Limiter
            rateLimiterOptions.GlobalLimiter = CreateGlobalLimiter(options);

            // Named Policies
            foreach (var policy in options.Policies)
            {
                rateLimiterOptions.AddPolicy<string>(policy.Key, context =>
                    CreatePartitionedLimiter(context, policy.Value));
            }

            // Default Policy
            rateLimiterOptions.AddPolicy<string>("Default", context =>
                CreatePartitionedLimiter(context, options.DefaultPolicy));

            // OnRejected Callback
            if (options.OnRejected is not null)
            {
                rateLimiterOptions.OnRejected = async (context, token) =>
                {
                    var httpContext = context.HttpContext;

                    // ✅ دریافت RetryAfter از Metadata
                    TimeSpan? retryAfter = null;
                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out TimeSpan retryAfterValue))
                    {
                        retryAfter = retryAfterValue;
                    }

                    var retryAfterSeconds = (int)(retryAfter?.TotalSeconds ?? 60);

                    // دریافت Key (با فرض اینکه Key را قبلاً در GetPartitionKey محاسبه کرده‌اید)
                    var policy = options.DefaultPolicy; // یا از منطق مناسب برای انتخاب Policy استفاده کنید
                    var key = GetPartitionKey(httpContext, policy);

                    await options.OnRejected(new RateLimitRejectedContext
                    {
                        HttpContext = httpContext,
                        Key = key,
                        RetryAfterSeconds = retryAfterSeconds,
                        Policy = policy
                    });
                };
            }
        });

        return services;
    }

    /// <summary>
    /// Adds the ImanSoftware Rate Limiter from IConfiguration.
    /// </summary>
    public static IServiceCollection AddImanRateLimiter(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var config = configuration.Get<RateLimiterConfiguration>() ?? new RateLimiterConfiguration();

        return services.AddImanRateLimiter(options =>
        {
            options.DefaultPolicy = config.DefaultPolicy ?? new RateLimitPolicy();
            options.EnableMetrics = config.EnableMetrics;
            options.EnableLogging = config.EnableLogging;

            foreach (var policy in config.Policies)
                options.Policies[policy.Key] = policy.Value;
        });
    }

    // Private: Creates the global partitioned limiter
    private static PartitionedRateLimiter<HttpContext> CreateGlobalLimiter(ImanRateLimiterOptions options)
    {
        var policy = options.DefaultPolicy;
        return PartitionedRateLimiter.Create<HttpContext, string>(
            context =>
            {
                var partitionKey = GetPartitionKey(context, policy);
                return CreateLimiterPartition(partitionKey, policy);
            });
    }

    // Private: Creates a named policy limiter (simplified)
    private static RateLimitPartition<string> CreatePartitionedLimiter(
        HttpContext context,
        RateLimitPolicy policy)
    {
        var partitionKey = GetPartitionKey(context, policy);
        return CreateLimiterPartition(partitionKey, policy);
    }

    // Private: Creates a rate limit partition based on the policy strategy
    private static RateLimitPartition<string> CreateLimiterPartition(
        string partitionKey,
        RateLimitPolicy policy)
    {
        return policy.Strategy switch
        {
            RateLimiterStrategy.FixedWindow => RateLimitPartition.GetFixedWindowLimiter(
                partitionKey,
                _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = policy.PermitLimit,
                    Window = policy.Window,
                    QueueLimit = policy.QueueLimit ?? 0
                }),

            RateLimiterStrategy.SlidingWindow => RateLimitPartition.GetSlidingWindowLimiter(
                partitionKey,
                _ => new SlidingWindowRateLimiterOptions
                {
                    PermitLimit = policy.PermitLimit,
                    Window = policy.Window,
                    SegmentsPerWindow = 10,
                    QueueLimit = policy.QueueLimit ?? 0
                }),

            RateLimiterStrategy.TokenBucket => RateLimitPartition.GetTokenBucketLimiter(
                partitionKey,
                _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = policy.PermitLimit,
                    QueueLimit = policy.QueueLimit ?? 0,
                    TokensPerPeriod = policy.TokensPerSecond ?? 10,
                    AutoReplenishment = true,
                    ReplenishmentPeriod = TimeSpan.FromSeconds(1)
                }),

            RateLimiterStrategy.Concurrency => RateLimitPartition.GetConcurrencyLimiter(
                partitionKey,
                _ => new ConcurrencyLimiterOptions
                {
                    PermitLimit = policy.PermitLimit,
                    QueueLimit = policy.QueueLimit ?? 0
                }),

            _ => RateLimitPartition.GetFixedWindowLimiter(
                partitionKey,
                _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = policy.PermitLimit,
                    Window = policy.Window
                })
        };
    }

    // Private: Gets the partition key from HttpContext
    private static string GetPartitionKey(HttpContext context, RateLimitPolicy policy)
    {
        return policy.PartitionBy switch
        {
            RateLimitPartitionBy.IpAddress => context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            RateLimitPartitionBy.UserId => context.User.FindFirst("sub")?.Value
                                          ?? context.User.FindFirst("nameid")?.Value
                                          ?? "anonymous",
            RateLimitPartitionBy.ApiKey => context.Request.Headers["X-API-Key"].FirstOrDefault() ?? "unknown",
            RateLimitPartitionBy.Endpoint => context.Request.Path.ToString(),
            RateLimitPartitionBy.Custom => policy.CustomPartitionResolver?.Invoke(context) ?? "custom",
            _ => "default"
        };
    }
}