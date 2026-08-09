namespace ImanSoftware.RateLimiter;

/// <summary>
/// Configuration options for the rate limiter.
/// </summary>
public class RateLimiterOptions
{
    public string DefaultStrategy { get; set; } = "FixedWindow";
    public RateLimitPolicy DefaultPolicy { get; set; } = new() { Limit = 100, WindowSeconds = 60 };
    public Dictionary<string, RateLimitPolicy> Policies { get; set; } = new();
}