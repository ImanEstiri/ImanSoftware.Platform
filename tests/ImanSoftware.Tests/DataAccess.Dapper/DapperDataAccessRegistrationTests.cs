using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using ImanSoftware.DataAccess;
using ImanSoftware.DataAccess.Dapper;

namespace ImanSoftware.Tests.DataAccess.Dapper;

public class DapperDataAccessRegistrationTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void AddDapperDataAccess_WithInvalidConnectionString_ThrowsArgumentException(string? connectionString)
    {
        var services = new ServiceCollection();

        var exception = Assert.Throws<ArgumentException>(
            () => services.AddDapperDataAccess(connectionString!));

        // .NET appends "(Parameter 'connectionString')" to the message.
        Assert.StartsWith("Connection string is required.", exception.Message, StringComparison.Ordinal);
        Assert.Equal("connectionString", exception.ParamName);
    }

    [Fact]
    public void AddDapperDataAccess_RegistersConnectionFactoryAsSingletonInstance()
    {
        var services = new ServiceCollection();

        services.AddDapperDataAccess("Server=.;Database=Test");

        var descriptor = Assert.Single(services, d => d.ServiceType == typeof(IDbConnectionFactory));
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [Fact]
    public void AddDapperDataAccess_RegistersDataAccessorAsScoped()
    {
        var services = new ServiceCollection();

        services.AddDapperDataAccess("Server=.;Database=Test");

        var descriptor = Assert.Single(services, d => d.ServiceType == typeof(ISqlDataAccessor));
        Assert.Equal(typeof(DapperDataAccessor), descriptor.ImplementationType);
        Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [Fact]
    public void AddDapperDataAccess_TryAdd_PreservesExistingFactory()
    {
        var services = new ServiceCollection();
        var existingFactory = new DapperConnectionFactory("Server=.;Database=Existing");
        services.AddSingleton<IDbConnectionFactory>(existingFactory);

        services.AddDapperDataAccess("Server=.;Database=New");

        var descriptor = Assert.Single(services, d => d.ServiceType == typeof(IDbConnectionFactory));
        Assert.Same(existingFactory, descriptor.ImplementationInstance);
    }

    [Fact]
    public void AddDapperDataAccess_ResolvesConfiguredFactoryAndAccessor()
    {
        var services = new ServiceCollection();

        services.AddDapperDataAccess("Server=.;Database=Test");

        using var provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<IDbConnectionFactory>();
        var accessor = provider.GetRequiredService<ISqlDataAccessor>();

        Assert.IsType<DapperConnectionFactory>(factory);
        Assert.Equal("Server=.;Database=Test", factory.ConnectionString);
        Assert.IsType<DapperDataAccessor>(accessor);
        Assert.Same(factory, ((DapperDataAccessor)accessor).ConnectionFactory);
    }

    [Fact]
    public void AddDapperDataAccess_ReturnsSameServices()
    {
        var services = new ServiceCollection();

        Assert.Same(services, services.AddDapperDataAccess("Server=.;Database=Test"));
    }
}
