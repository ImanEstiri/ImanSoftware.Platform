// مسیر: Core/RateLimiterStrategyType.cs
namespace ImanSoftware.RateLimiter;

/// <summary>
/// Supported rate limiting strategies.
/// </summary>
public enum RateLimiterStrategyType
{
    /// <summary>
    /// Fixed window strategy (e.g., 100 requests per minute, resets every minute).
    /// </summary>
    FixedWindow,

    /// <summary>
    /// Sliding window strategy (more accurate than fixed window, uses a rolling time window).
    /// </summary>
    //SlidingWindow,

    /// <summary>
    /// Token bucket strategy (allows bursts up to capacity, refills at a fixed rate).
    /// </summary>
    //TokenBucket
}