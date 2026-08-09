using ImanSoftware.RateLimiter.Core.Models;
using ImanSoftware.RateLimiter.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ImanSoftware.RateLimiter;

/// <summary>
/// Extension methods for adding the rate limiter middleware.
/// </summary>
public static class UseRateLimiterExtensions
{
    /// <summary>
    /// Adds the ImanSoftware Rate Limiter middleware to the pipeline.
    /// </summary>
    public static IApplicationBuilder UseImanRateLimiter(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        // Verify that the rate limiter services are registered
        using var scope = app.ApplicationServices.CreateScope();
        var options = scope.ServiceProvider.GetService<ImanRateLimiterOptions>();
        if (options is null)
        {
            throw new InvalidOperationException(
                "RateLimiterOptions is not registered. Call AddImanRateLimiter() in Program.cs.");
        }

        // Add the Microsoft rate limiter middleware first
        app.UseRateLimiter();

        // Add custom middleware for headers and logging
        app.UseMiddleware<RateLimiterMiddleware>();

        return app;
    }
}