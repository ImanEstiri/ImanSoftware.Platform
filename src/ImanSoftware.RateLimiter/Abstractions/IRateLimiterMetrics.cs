namespace ImanSoftware.RateLimiter.Abstractions;

/// <summary>
/// Contract for rate limiter metrics.
/// </summary>
public interface IRateLimiterMetrics
{
    /// <summary>
    /// Records an allowed request.
    /// </summary>
    void RecordAllowed(string policyName, string key);

    /// <summary>
    /// Records a rejected request.
    /// </summary>
    void RecordRejected(string policyName, string key, int retryAfterSeconds);
}