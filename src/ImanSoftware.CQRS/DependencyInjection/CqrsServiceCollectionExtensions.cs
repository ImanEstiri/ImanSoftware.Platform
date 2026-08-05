// مسیر: DependencyInjection/CqrsServiceCollectionExtensions.cs
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ImanSoftware.CQRS.DependencyInjection;

public static class CqrsServiceCollectionExtensions
{
    /// <summary>
    /// Adds CQRS (MediatR) services to the DI container.
    /// </summary>
    public static IServiceCollection AddCqrs(
        this IServiceCollection services,
        Assembly[] assemblies,
        Action<MediatRServiceConfiguration>? configure = null)
    {
        var configuration = new MediatRServiceConfiguration();
        configuration.RegisterServicesFromAssemblies(assemblies);

        configure?.Invoke(configuration);

        services.AddMediatR(configuration);

        return services;
    }
}