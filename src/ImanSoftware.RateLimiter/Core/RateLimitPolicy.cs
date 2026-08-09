namespace ImanSoftware.RateLimiter;

/// <summary>
/// Represents a rate limit policy (e.g., 100 requests per minute).
/// </summary>
public record RateLimitPolicy
{
    public int Limit { get; init; }          // Maximum number of requests
    public int WindowSeconds { get; init; }  // Time window in seconds
    public RateLimiterStrategyType Strategy { get; init; } = RateLimiterStrategyType.FixedWindow;  // "FixedWindow", "SlidingWindow", "TokenBucket"
}