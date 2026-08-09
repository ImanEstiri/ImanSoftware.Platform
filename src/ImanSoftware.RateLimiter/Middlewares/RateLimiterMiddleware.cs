using Microsoft.AspNetCore.Http;
using ImanSoftware.RateLimiter;

public class RateLimiterMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IRateLimiter _rateLimiter;

    public RateLimiterMiddleware(RequestDelegate next, IRateLimiter rateLimiter)
    {
        _next = next;
        _rateLimiter = rateLimiter;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var key = context.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                  ?? context.Connection.RemoteIpAddress?.ToString()
                  ?? "unknown";

        var result = await _rateLimiter.IsRequestAllowedAsync(key);

        if (!result.IsAllowed)
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.Response.Headers["Retry-After"] = result.RetryAfterSeconds.ToString();
            await context.Response.WriteAsync($"Rate limit exceeded. Retry after {result.RetryAfterSeconds} seconds.");
            return;
        }

        context.Response.Headers["X-RateLimit-Remaining"] = result.Remaining.ToString();
        await _next(context);
    }
}