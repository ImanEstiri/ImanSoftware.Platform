namespace ImanSoftware.RateLimiter;

/// <summary>
/// Contract for rate limiting functionality.
/// </summary>
public interface IRateLimiter
{
    /// <summary>
    /// Checks if a request is allowed based on the current rate limit policy.
    /// </summary>
    Task<RateLimitResult> IsRequestAllowedAsync(string key, CancellationToken cancellationToken = default);
}