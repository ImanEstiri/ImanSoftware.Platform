using Microsoft.AspNetCore.Builder;

namespace ImanSoftware.Framework.AspNetCore.Middlewares;

public static class UseMiddlewareExtensions
{
    public static IApplicationBuilder UseJwtAuthentication(this IApplicationBuilder app)
    {
        return app.UseMiddleware<JwtMiddleware>();
    }
}
