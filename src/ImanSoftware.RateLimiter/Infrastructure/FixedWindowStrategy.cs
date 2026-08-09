namespace ImanSoftware.RateLimiter.Infrastructure;

/// <summary>
/// Fixed window rate limiting strategy (e.g., 100 requests per minute, resets every minute).
/// </summary>
public class FixedWindowStrategy : IRateLimiterStrategy
{
    public (bool Allowed, int Remaining, int RetryAfterSeconds) ShouldAllow(string key, RateLimitPolicy policy, object? state)
    {
        var (count, windowStart) = ((int Count, DateTime WindowStart))state!;
        var now = DateTime.UtcNow;
        var windowSeconds = policy.WindowSeconds;

        if ((now - windowStart).TotalSeconds >= windowSeconds)
        {
            // Reset window
            return (true, policy.Limit - 1, 0);
        }

        if (count >= policy.Limit)
        {
            var retryAfter = (int)(windowSeconds - (now - windowStart).TotalSeconds);
            return (false, 0, retryAfter);
        }

        return (true, policy.Limit - count - 1, 0);
    }
}