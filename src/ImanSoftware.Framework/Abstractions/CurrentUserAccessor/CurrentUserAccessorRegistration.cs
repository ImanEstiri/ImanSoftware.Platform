
using ImanSoftware.Framework.Infrastructure.CurrentUserAccessor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ImanSoftware.Framework.Abstractions.CurrentUserAccessor;

internal static class CurrentUserAccessorRegistration
{

    internal static IServiceCollection AddCurrentUserAccessor(
        this IServiceCollection services, Action<IServiceCollection>? customImplementation = null)
    {
        services.AddHttpContextAccessor();
        if (customImplementation is not null)
        {
            customImplementation(services);
        }
        else
        {
            services.TryAddSingleton(typeof(ICurrentUserAccessor<>), typeof(HttpContextUserAccessor<>));
        }

        return services;
    }

}
