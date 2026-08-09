namespace ImanSoftware.RateLimiter;

/// <summary>
/// Represents the result of a rate limit check.
/// </summary>
public record RateLimitResult
{
    public bool IsAllowed { get; init; }
    public int Remaining { get; init; }
    public int RetryAfterSeconds { get; init; }
    public string? Reason { get; init; }

    public static RateLimitResult Allowed(int remaining) =>
        new() { IsAllowed = true, Remaining = remaining };

    public static RateLimitResult Denied(int retryAfterSeconds, string? reason = null) =>
        new() { IsAllowed = false, RetryAfterSeconds = retryAfterSeconds, Reason = reason };
}