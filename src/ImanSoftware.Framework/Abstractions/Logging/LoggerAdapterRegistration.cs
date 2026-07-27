using ImanSoftware.Framework.Infrastructure.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ImanSoftware.Framework.Abstractions.Logging;

internal static class LoggerAdapterRegistration
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
            // ثبت Open Generic برای LoggerAdapter
            // ILoggerAdapter<> به LoggerAdapter<> نگاشت می‌شود
            services.TryAddSingleton(typeof(ILoggerAdapter<>), typeof(LoggerAdapter<>));
        }

        return services;
    }
}