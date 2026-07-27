using ImanSoftware.Framework.Abstractions.Jwt;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ImanSoftware.Framework.AspNetCore.Middlewares;

public static class UseMiddlewareExtensions
{
    public static IApplicationBuilder UseJwtAuthentication(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var tokenService = scope.ServiceProvider.GetService<IJwtTokenService>();

        if (tokenService is null)
        {
            throw new InvalidOperationException("IJwtTokenService is not registered in the DI container. ");
        }

        return app.UseMiddleware<JwtMiddleware>();
    }
}
