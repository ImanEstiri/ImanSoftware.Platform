using ImanSoftware.Logging;
using ImanSoftware.RateLimiter.Core.Models;
using Microsoft.AspNetCore.Http;

namespace ImanSoftware.RateLimiter.Middlewares;

/// <summary>
/// Middleware for adding rate limit headers and logging.
/// </summary>
public class RateLimiterMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ImanRateLimiterOptions _options;
    private readonly ILoggerAdapter<RateLimiterMiddleware>? _logger;

    public RateLimiterMiddleware(
        RequestDelegate next,
        ImanRateLimiterOptions options,
        ILoggerAdapter<RateLimiterMiddleware>? logger = null)
    {
        _next = next;
        _options = options;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Store original response stream
        var originalBodyStream = context.Response.Body;

        // Use a memory stream to capture the response
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        // Continue pipeline
        await _next(context);

        // Check if rate limit headers should be added
        if (_options.DefaultPolicy.IncludeHeaders)
        {
            var headers = ExtractRateLimitHeaders(context);
            if (headers is not null)
            {
                headers.ApplyTo(context.Response);
                context.Response.Headers["X-RateLimit-Policy"] = _options.DefaultPolicy.Strategy.ToString();
            }
        }

        // Log the request
        if (_options.EnableLogging)
        {
            _logger?.LogInformation(
                "Request {Method} {Path} - Status: {StatusCode}, IP: {IP}",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                context.Connection.RemoteIpAddress?.ToString() ?? "unknown");
        }

        // Copy the response back to the original stream
        responseBody.Seek(0, SeekOrigin.Begin);
        await responseBody.CopyToAsync(originalBodyStream);
    }

    private static RateLimitHeaders? ExtractRateLimitHeaders(HttpContext context)
    {
        // Try to extract headers from the rate limiter lease
        // Note: The actual implementation depends on how you track the lease
        // This is a placeholder
        return null;
    }
}