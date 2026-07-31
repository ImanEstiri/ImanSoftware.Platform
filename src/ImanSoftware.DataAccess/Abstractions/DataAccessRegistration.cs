using Microsoft.Extensions.DependencyInjection;

namespace ImanSoftware.DataAccess;

public static class DataAccessRegistration
{
    public static IServiceCollection AddDataAccess(
        this IServiceCollection services,
        string connectionString,
        Action<IServiceCollection>? customRegistration = null)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Connection string is required.", nameof(connectionString));

        if (customRegistration is not null)
        {
            customRegistration(services);
        }
        else
        {
            //services.TryAddSingleton<IDbConnectionFactory>(new DapperConnectionFactory(connectionString));
            //services.TryAddScoped<ISqlDataAccessor, DapperDataAccessor>();
        }

        return services;
    }
}