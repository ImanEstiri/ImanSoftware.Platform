using Microsoft.AspNetCore.Http;

namespace ImanSoftware.RateLimiter.Core.Models;

/// <summary>
/// Standard rate limit response headers.
/// </summary>
public record RateLimitHeaders
{
    public int Limit { get; init; }
    public int Remaining { get; init; }
    public DateTimeOffset Reset { get; init; }

    public void ApplyTo(HttpResponse response)
    {
        response.Headers["X-RateLimit-Limit"] = Limit.ToString();
        response.Headers["X-RateLimit-Remaining"] = Remaining.ToString();
        response.Headers["X-RateLimit-Reset"] = Reset.ToUnixTimeSeconds().ToString();
    }
}