using Microsoft.AspNetCore.Http;

namespace ImanSoftware.RateLimiter.Core.Models;

/// <summary>
/// Configuration options for the ImanSoftware Rate Limiter.
/// </summary>
public class ImanRateLimiterOptions
{
    /// <summary>
    /// Default policy applied to all endpoints.
    /// </summary>
    public RateLimitPolicy DefaultPolicy { get; set; } = new();

    /// <summary>
    /// Named policies for specific endpoints or controllers.
    /// </summary>
    public Dictionary<string, RateLimitPolicy> Policies { get; set; } = new();

    /// <summary>
    /// Whether to enable built-in metrics.
    /// </summary>
    public bool EnableMetrics { get; set; } = true;

    /// <summary>
    /// Whether to enable logging for rate limit events.
    /// </summary>
    public bool EnableLogging { get; set; } = true;

    /// <summary>
    /// Custom logging handler.
    /// </summary>
    public Action<RateLimitEvent>? LogHandler { get; set; }

    /// <summary>
    /// Custom on-rejected handler.
    /// </summary>
    public Func<RateLimitRejectedContext, Task>? OnRejected { get; set; }
}

/// <summary>
/// Rate limit event for logging.
/// </summary>
public record RateLimitEvent
{
    public string Key { get; init; } = string.Empty;
    public bool IsAllowed { get; init; }
    public int Remaining { get; init; }
    public int RetryAfterSeconds { get; init; }
    public string PolicyName { get; init; } = "Default";
    public DateTimeOffset Timestamp { get; init; }
}

/// <summary>
/// Context for on-rejected callback.
/// </summary>
public record RateLimitRejectedContext
{
    public HttpContext HttpContext { get; init; } = default!;
    public string Key { get; init; } = string.Empty;
    public int RetryAfterSeconds { get; init; }
    public RateLimitPolicy Policy { get; init; } = new();
}