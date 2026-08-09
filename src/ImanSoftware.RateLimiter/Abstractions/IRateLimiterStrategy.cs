namespace ImanSoftware.RateLimiter;

/// <summary>
/// Defines the strategy for rate limiting (e.g., Fixed Window, Sliding Window, Token Bucket).
/// </summary>
public interface IRateLimiterStrategy
{
    /// <summary>
    /// Determines if a request is allowed and returns the updated state.
    /// </summary>
    (bool Allowed, int Remaining, int RetryAfterSeconds) ShouldAllow(string key, RateLimitPolicy policy, object? state = null);
}