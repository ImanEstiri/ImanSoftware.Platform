using Microsoft.Extensions.DependencyInjection;

namespace ImanSoftware.DataAccess;

public static class DataAccessRegistration
{
    public static IServiceCollection AddDataAccess(
        this IServiceCollection services,
        Action<IServiceCollection> customRegistration)
    {
        ArgumentNullException.ThrowIfNull(customRegistration , paramName: "CustomRegistration for ImanSoftware.DataAccess");

        customRegistration(services);

        return services;
    }
}