using ImanSoftware.Framework.Abstractions.Caching;
using ImanSoftware.Framework.Abstractions.CurrentUserAccessor;
using ImanSoftware.Framework.Abstractions.DataAccess;
using ImanSoftware.Framework.Abstractions.DateTimeProvider;
using ImanSoftware.Framework.Abstractions.GuidGenerator;
using ImanSoftware.Framework.Abstractions.Jwt;
using ImanSoftware.Framework.Abstractions.Serialization;
using ImanSoftware.Framework.Infrastructure.Jwt;
using Microsoft.Extensions.DependencyInjection;

namespace ImanSoftware.Framework.FrameworkDependencyInjection;


/// <summary>
/// Central entry point for registering all ImanFramework services.
/// Uses internal registration classes from each abstraction module.
/// </summary>
public static class FrameworkDependencyInjection
{
    /// <summary>
    /// Registers all enabled ImanFramework services into the DI container.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="options">Configuration options for the framework.</param>
    /// <returns>The modified IServiceCollection for chaining.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when a service is enabled but no registration (default or custom) is available.
    /// </exception>
    public static IServiceCollection AddImanFramework(
        this IServiceCollection services,
        ImanFrameworkDependencyInjectionOptions options)
    {
        if (options is null)
            options = new ImanFrameworkDependencyInjectionOptions();

        // 1. DateTime Provider
        if (options.AddDateTimeProvider)
        {
            DateTimeProviderRegistration.AddDateTimeProvider(
                services,
                options.DateTimeProviderCustomImplementation?.CustomImplementation);
        }

        // 2. Guid Generator
        if (options.AddGuidGenerator)
        {
            GuidGeneratorRegistration.AddGuidGenerator(
                services,
                options.GuidGeneratorImplementationType,
                options.GuidGeneratorCustomImplementation?.CustomImplementation);
        }

        // 3. Current User Accessor - Default Is Open Generic (Must have custom registration)
        if (options.AddCurrentUserAccessor)
        {
            CurrentUserAccessorRegistration.AddCurrentUserAccessor(
                services,
                options.CurrentUserAccessorCustomRegistration);
        }

        // 4. JsonSerializer
        if (options.AddJsonSerializer)
        {
            JsonSerializerRegistration.AddJsonSerializer(
                services,
                options.SerializationImplementationType,
                options.JsonSerializerCustomImplementation?.CustomImplementation);
        }

        // 5. CacheService
        if (options.AddCacheService)
        {
            CacheServiceRegistration.AddCacheService(
                services,
                options.CacheServiceCustomImplementation?.CustomImplementation);
        }

        // 6. JwtService
        if (options.AddJwtService)
        {
            if (options.JwtCustomImplementation is null && options.JwtConfigureOptions is null)
            {
                throw new ArgumentException(message: "ImanSoftware.Abstractions.Jwt.IJwtService Needs Options For Registering Default Implementation.", paramName: nameof(options.JwtConfigureOptions));
            }
            JwtTokenServiceRegistration.AddJwtTokenService(
                services,
                options.JwtConfigureOptions,
                options.JwtCustomImplementation?.CustomImplementation);
        }

        // 7. DataAccess
        if (options.AddDataAccess)
        {
            if (options.DataAccessCustomRegistration is null )
            {
                // When using the default DataAccess implementation, ConnectionString must be provided.
                ArgumentNullException.ThrowIfNullOrWhiteSpace(options.ConnectionString, nameof(options.ConnectionString));
            }

            DataAccessRegistration.AddDataAccess(
                services,
                options.ConnectionString!,
                options.DataAccessCustomRegistration);
        }

        return services;
    }
}