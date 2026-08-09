using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ImanSoftware.RateLimiter;

public static class UseRateLimiterExtensions
{
    public static IApplicationBuilder UseRateLimiter(this IApplicationBuilder app)
    {
        // بررسی وجود سرویس در DI (اختیاری ولی مفید)
        using var scope = app.ApplicationServices.CreateScope();
        var rateLimiter = scope.ServiceProvider.GetService<IRateLimiter>();
        if (rateLimiter is null)
            throw new InvalidOperationException("IRateLimiter is not registered. Call AddRateLimiter() in Program.cs.");

        return app.UseMiddleware<RateLimiterMiddleware>();
    }
}