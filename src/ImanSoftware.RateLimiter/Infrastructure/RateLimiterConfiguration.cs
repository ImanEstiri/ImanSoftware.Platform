using ImanSoftware.RateLimiter.Core.Models;

namespace ImanSoftware.RateLimiter.Infrastructure;

/// <summary>
/// Configuration model for appsettings.json.
/// </summary>
public class RateLimiterConfiguration
{
    public RateLimitPolicy? DefaultPolicy { get; set; }
    public Dictionary<string, RateLimitPolicy> Policies { get; set; } = new();
    public bool EnableMetrics { get; set; } = true;
    public bool EnableLogging { get; set; } = true;
}