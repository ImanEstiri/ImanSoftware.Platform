namespace ImanSoftware.RateLimiter.Core.Enums;

/// <summary>
/// Defines the rate limiting strategy algorithms.
/// </summary>
public enum RateLimiterStrategy
{
    /// <summary>
    /// Fixed window strategy: limits requests within a fixed time window.
    /// </summary>
    FixedWindow,

    /// <summary>
    /// Sliding window strategy: limits requests within a sliding time window.
    /// </summary>
    SlidingWindow,

    /// <summary>
    /// Token bucket strategy: tokens are refilled at a fixed rate.
    /// </summary>
    TokenBucket,

    /// <summary>
    /// Concurrency strategy: limits the number of concurrent requests.
    /// </summary>
    Concurrency
}