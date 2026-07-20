using ImanSoftware.Framework.Infrastructure.DateTimeProvider;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ImanSoftware.Framework.Abstractions.DateTimeProvider;

internal static class DateTimeProviderRegistration
{
    public static IServiceCollection AddDateTimeProvider(
    this IServiceCollection services, Type? customImplementation = null)
    {
        if (customImplementation is not null)
            services.AddSingleton(typeof(IDateTimeProvider), customImplementation);
        else
            services.TryAddSingleton<IDateTimeProvider, DefaultDateTimeProvider>();

        return services;
    }
}
