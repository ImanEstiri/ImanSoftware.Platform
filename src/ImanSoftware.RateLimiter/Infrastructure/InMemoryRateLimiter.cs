using System.Collections.Concurrent;

namespace ImanSoftware.RateLimiter.Infrastructure;

/// <summary>
/// In-memory implementation of IRateLimiter.
/// </summary>
public class InMemoryRateLimiter : IRateLimiter
{
    private readonly RateLimiterOptions _options;
    private readonly ConcurrentDictionary<string, (int Count, DateTime WindowStart)> _state = new();
    private readonly IRateLimiterStrategy _strategy;

    public InMemoryRateLimiter(RateLimiterOptions options, IRateLimiterStrategy strategy)
    {
        _options = options;
        _strategy = strategy;
    }

    public Task<RateLimitResult> IsRequestAllowedAsync(string key, CancellationToken cancellationToken = default)
    {
        var policy = _options.Policies.GetValueOrDefault(key, _options.DefaultPolicy);
        var state = _state.GetOrAdd(key, _ => (0, DateTime.UtcNow));

        var (allowed, remaining, retryAfter) = _strategy.ShouldAllow(key, policy, state);
        return Task.FromResult(allowed
            ? RateLimitResult.Allowed(remaining)
            : RateLimitResult.Denied(retryAfter));
    }
}