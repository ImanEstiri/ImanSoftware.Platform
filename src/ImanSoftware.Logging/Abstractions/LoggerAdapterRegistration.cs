using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ImanSoftware.Logging;

public static class LoggerAdapterRegistration
{
    public static IServiceCollection AddLoggerAdapter(
        this IServiceCollection services,
        Action<IServiceCollection>? customRegistration = null)
    {
        if (customRegistration is not null)
        {
            customRegistration(services);
        }
        else
        {
            services.TryAddSingleton(typeof(ILoggerAdapter<>), typeof(LoggerAdapter<>));
        }

        return services;
    }
}