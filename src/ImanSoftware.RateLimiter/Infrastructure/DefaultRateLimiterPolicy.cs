using ImanSoftware.RateLimiter.Core.Models;
using Microsoft.AspNetCore.Http;

namespace ImanSoftware.RateLimiter.Infrastructure;

/// <summary>
/// Default implementation of IRateLimiterPolicy.
/// </summary>
public class DefaultRateLimiterPolicy : IRateLimiterPolicy
{
    private readonly ImanRateLimiterOptions _options;

    public DefaultRateLimiterPolicy(ImanRateLimiterOptions options)
    {
        _options = options;
    }

    public string GetPartitionKey(HttpContext context)
    {
        var policy = _options.DefaultPolicy;
        return policy.PartitionBy switch
        {
            Core.Enums.RateLimitPartitionBy.IpAddress => context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            Core.Enums.RateLimitPartitionBy.UserId => context.User.FindFirst("sub")?.Value ?? "anonymous",
            Core.Enums.RateLimitPartitionBy.ApiKey => context.Request.Headers["X-API-Key"].FirstOrDefault() ?? "unknown",
            Core.Enums.RateLimitPartitionBy.Endpoint => context.Request.Path.ToString(),
            Core.Enums.RateLimitPartitionBy.Custom => policy.CustomPartitionResolver?.Invoke(context) ?? "custom",
            _ => "default"
        };
    }

    public RateLimitPolicy GetPolicy(HttpContext context)
    {
        return _options.DefaultPolicy;
    }
}