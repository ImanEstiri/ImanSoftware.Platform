using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ImanSoftware.DataAccess.Dapper; // Namespace یکسان با بقیه‌ی کلاس‌های Dapper

/// <summary>
/// Registration extensions for Dapper implementation of DataAccess.
/// </summary>
public static class DapperDataAccessRegistration
{
    /// <summary>
    /// Registers the Dapper implementation of ISqlDataAccessor and IDbConnectionFactory.
    /// </summary>
    /// <param name="services">The IServiceCollection.</param>
    /// <param name="connectionString">The database connection string.</param>
    /// <returns>The modified IServiceCollection.</returns>
    public static IServiceCollection AddDapperDataAccess(
        this IServiceCollection services,
        string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Connection string is required.", nameof(connectionString));

        return services.AddDataAccess(innerServices =>
        {
            innerServices.TryAddSingleton<IDbConnectionFactory>(new DapperConnectionFactory(connectionString));
            innerServices.TryAddScoped<ISqlDataAccessor, DapperDataAccessor>();
        });

    }
}