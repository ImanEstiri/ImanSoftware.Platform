using ImanSoftware.RateLimiter.Core.Models;
using Microsoft.AspNetCore.Http;

namespace ImanSoftware.RateLimiter;

/// <summary>
/// Contract for custom rate limiter policies.
/// </summary>
public interface IRateLimiterPolicy
{
    /// <summary>
    /// Gets the partition key for the given HttpContext.
    /// </summary>
    string GetPartitionKey(HttpContext context);

    /// <summary>
    /// Gets the rate limit policy for the given context.
    /// </summary>
    RateLimitPolicy GetPolicy(HttpContext context);
}