using ImanSoftware.Logging;
using ImanSoftware.RateLimiter.Core.Enums;
using ImanSoftware.RateLimiter.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.RateLimiting;

namespace ImanSoftware.RateLimiter.Attributes;

/// <summary>
/// Attribute to enable rate limiting on a controller or endpoint.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class EnableImanRateLimitingAttribute : Attribute, IFilterFactory
{
    public string PolicyName { get; set; } = "Default";

    public bool IsReusable => false;

    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        var options = serviceProvider.GetRequiredService<ImanRateLimiterOptions>();
        var logger = serviceProvider.GetService<ILoggerAdapter<EnableImanRateLimitingFilter>>();

        return new EnableImanRateLimitingFilter(PolicyName, options, logger);
    }
}

/// <summary>
/// Filter implementation for rate limiting.
/// </summary>
public class EnableImanRateLimitingFilter : IAsyncActionFilter
{
    private readonly string _policyName;
    private readonly ImanRateLimiterOptions _options;
    private readonly ILoggerAdapter<EnableImanRateLimitingFilter>? _logger;

    public EnableImanRateLimitingFilter(
        string policyName,
        ImanRateLimiterOptions options,
        ILoggerAdapter<EnableImanRateLimitingFilter>? logger)
    {
        _policyName = policyName;
        _options = options;
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Check if the policy exists
        var policy = _policyName == "Default"
            ? _options.DefaultPolicy
            : _options.Policies.GetValueOrDefault(_policyName);

        if (policy is null)
        {
            _logger?.LogWarning("Rate limit policy '{PolicyName}' not found. Using default.", _policyName);
            policy = _options.DefaultPolicy;
        }

        var httpContext = context.HttpContext;
        var limiter = GetLimiterForPolicy(_policyName, policy);

        var lease = await limiter.AcquireAsync(httpContext, 1);

        if (lease is not null && !lease.IsAcquired)
        {
            // دریافت RetryAfter از Metadata
            TimeSpan? retryAfter = null;
            if (lease.TryGetMetadata(MetadataName.RetryAfter, out TimeSpan retryAfterValue))
            {
                retryAfter = retryAfterValue;
            }

            var retryAfterSeconds = (int)(retryAfter?.TotalSeconds ?? 60);

            // تنظیم هدرها در پاسخ رد شده
            if (policy.IncludeHeaders)
            {
                httpContext.Response.Headers["X-RateLimit-Limit"] = policy.PermitLimit.ToString();
                httpContext.Response.Headers["X-RateLimit-Remaining"] = "0";
                httpContext.Response.Headers["Retry-After"] = retryAfterSeconds.ToString();
            }

            context.Result = new ObjectResult(new
            {
                Error = "Rate limit exceeded",
                RetryAfter = retryAfterSeconds,
                Policy = _policyName,
                Limit = policy.PermitLimit,
                Window = policy.Window.TotalSeconds
            })
            {
                StatusCode = StatusCodes.Status429TooManyRequests
            };

            return;
        }

        // Request allowed - add remaining count if available
        if (policy.IncludeHeaders && lease is not null)
        {
            // ✅ دریافت تعداد مجوزهای باقی‌مانده از متادیتا
            int? remaining = GetRemainingTokens(lease,policy);

            var remainingAfter = (remaining ?? policy.PermitLimit) - 1;
            httpContext.Response.Headers["X-RateLimit-Remaining"] = Math.Max(0, remainingAfter).ToString();
            httpContext.Response.Headers["X-RateLimit-Limit"] = policy.PermitLimit.ToString();
            httpContext.Response.Headers["X-RateLimit-Policy"] = policy.Strategy.ToString();
        }

        await next();
    }

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

    private static PartitionedRateLimiter<HttpContext> GetLimiterForPolicy(
        string policyName,
        RateLimitPolicy policy)
    {
        return PartitionedRateLimiter.Create<HttpContext, string>(
            context =>
            {
                var partitionKey = GetPartitionKey(context, policy);
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
            });
    }

    /// <summary>
    /// Tries to get remaining tokens from the lease metadata using common keys.
    /// </summary>
    private static int? GetRemainingTokens(RateLimitLease lease, RateLimitPolicy policy)
    {
        // ابتدا سعی می‌کنیم متادیتا را بگیریم
        if (lease.TryGetMetadata("TotalTokens", out object? totalTokensObj) && totalTokensObj is int totalTokens)
            return totalTokens;

        // اگر نداشت، از استراتژی استفاده می‌کنیم
        return policy.Strategy switch
        {
            RateLimiterStrategy.TokenBucket or RateLimiterStrategy.Concurrency => policy.PermitLimit,
            _ => null
        };
    }
}