using System.Collections.Concurrent;
using ImanSoftware.RateLimiter.Abstractions;

namespace ImanSoftware.RateLimiter.Infrastructure;

/// <summary>
/// In-memory metrics implementation for rate limiter.
/// </summary>
public class RateLimiterMetrics : IRateLimiterMetrics
{
    private readonly ConcurrentDictionary<string, (long Allowed, long Rejected)> _counters = new();

    public void RecordAllowed(string policyName, string key)
    {
        var counter = _counters.GetOrAdd($"{policyName}:{key}", _ => (0, 0));
        _counters[$"{policyName}:{key}"] = (counter.Allowed + 1, counter.Rejected);
    }

    public void RecordRejected(string policyName, string key, int retryAfterSeconds)
    {
        var counter = _counters.GetOrAdd($"{policyName}:{key}", _ => (0, 0));
        _counters[$"{policyName}:{key}"] = (counter.Allowed, counter.Rejected + 1);
    }

    public (long Allowed, long Rejected) GetMetrics(string policyName, string key)
    {
        return _counters.GetValueOrDefault($"{policyName}:{key}", (0, 0));
    }
}