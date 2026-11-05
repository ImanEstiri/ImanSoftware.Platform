using Microsoft.Extensions.DependencyInjection;
using ImanSoftware.DataAccess;

namespace ImanSoftware.Tests.DataAccess;

public class DataAccessRegistrationTests
{
    [Fact]
    public void AddDataAccess_WithNullCustomRegistration_ThrowsArgumentNullExceptionWithCustomParamName()
    {
        var services = new ServiceCollection();

        var exception = Assert.Throws<ArgumentNullException>(() => services.AddDataAccess(null!));

        Assert.Equal("CustomRegistration for ImanSoftware.DataAccess", exception.ParamName);
    }

    [Fact]
    public void AddDataAccess_InvokesCustomRegistrationImmediately()
    {
        var services = new ServiceCollection();
        var invoked = false;

        services.AddDataAccess(s =>
        {
            invoked = true;
            s.AddSingleton<IDbConnectionFactory, StubConnectionFactory>();
        });

        Assert.True(invoked);
        Assert.Contains(services, d => d.ServiceType == typeof(IDbConnectionFactory));
    }

    [Fact]
    public void AddDataAccess_ReturnsSameServices()
    {
        var services = new ServiceCollection();

        Assert.Same(services, services.AddDataAccess(_ => { }));
    }

    private sealed class StubConnectionFactory : IDbConnectionFactory
    {
        public string ConnectionString => "stub";
        public System.Data.IDbConnection CreateConnection() => throw new NotSupportedException();
    }
}
