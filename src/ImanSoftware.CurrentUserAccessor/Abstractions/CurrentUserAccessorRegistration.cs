
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ImanSoftware.CurrentUserAccessor;

public static class CurrentUserAccessorRegistration
{

    public static IServiceCollection AddCurrentUserAccessor(
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
