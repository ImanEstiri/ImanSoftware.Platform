using ImanSoftware.RateLimiter.Core.Enums;
using Microsoft.AspNetCore.Http;

namespace ImanSoftware.RateLimiter.Core.Models;

/// <summary>
/// Represents a rate limiting policy.
/// </summary>
public class RateLimitPolicy
{
    /// <summary>
    /// Maximum number of requests allowed within the window.
    /// </summary>
    public int PermitLimit { get; set; } = 100;

    /// <summary>
    /// Time window for the rate limit.
    /// </summary>
    public TimeSpan Window { get; set; } = TimeSpan.FromMinutes(1);

    /// <summary>
    /// The strategy algorithm to use.
    /// </summary>
    public RateLimiterStrategy Strategy { get; set; } = RateLimiterStrategy.FixedWindow;

    /// <summary>
    /// The partition key for this policy.
    /// </summary>
    public RateLimitPartitionBy PartitionBy { get; set; } = RateLimitPartitionBy.IpAddress;

    /// <summary>
    /// Custom partition resolver (used when PartitionBy is Custom).
    /// </summary>
    public Func<HttpContext, string>? CustomPartitionResolver { get; set; }

    /// <summary>
    /// Whether to include rate limit headers in the response.
    /// </summary>
    public bool IncludeHeaders { get; set; } = true;

    /// <summary>
    /// Queue limit for Token Bucket and Concurrency strategies.
    /// </summary>
    public int? QueueLimit { get; set; }

    /// <summary>
    /// Auto-refill rate for Token Bucket strategy (tokens per second).
    /// </summary>
    public int? TokensPerSecond { get; set; }
}