using Microsoft.Extensions.DependencyInjection;

namespace ImanSoftware.Jwt;

public static class JwtTokenServiceRegistration
{
    public static IServiceCollection AddJwtTokenService(
        this IServiceCollection services,
        JwtOptions? configureOptions,
        Type? customImplementation = null)
    {
        if (customImplementation is not null)
        {
            services.AddSingleton(typeof(IJwtTokenService), customImplementation);
        }
        else
        {
            ArgumentNullException.ThrowIfNull(configureOptions, nameof(configureOptions));
            services.Configure<JwtOptions>(options =>
            {
                options.SecretKey = configureOptions.SecretKey;
                options.Issuer = configureOptions.Issuer;
                options.Audience = configureOptions.Audience;
                options.ExpiryMinutes = configureOptions.ExpiryMinutes;
                options.ValidateLifetime = configureOptions.ValidateLifetime;
            });
            services.AddSingleton<IJwtTokenService, JwtTokenService>();
        }

        return services;
    }
}