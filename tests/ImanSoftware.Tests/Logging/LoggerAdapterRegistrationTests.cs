using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ImanSoftware.Logging;

namespace ImanSoftware.Tests.Logging;

public class LoggerAdapterRegistrationTests
{
    private sealed class SampleService;

    [Fact]
    public void AddLoggerAdapter_RegistersOpenGenericAdapterAsSingleton()
    {
        var services = new ServiceCollection();

        services.AddLoggerAdapter();

        var descriptor = Assert.Single(services, d => d.ServiceType == typeof(ILoggerAdapter<>));
        Assert.Equal(typeof(LoggerAdapter<>), descriptor.ImplementationType);
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [Fact]
    public void AddLoggerAdapter_TryAdd_PreservesExistingRegistration()
    {
        var services = new ServiceCollection();
        services.AddSingleton(typeof(ILoggerAdapter<>), typeof(CustomLoggerAdapter<>));

        services.AddLoggerAdapter();

        var descriptor = Assert.Single(services, d => d.ServiceType == typeof(ILoggerAdapter<>));
        Assert.Equal(typeof(CustomLoggerAdapter<>), descriptor.ImplementationType);
    }

    [Fact]
    public void AddLoggerAdapter_WithCustomRegistration_InvokesDelegate()
    {
        var services = new ServiceCollection();
        var invoked = false;

        services.AddLoggerAdapter(s =>
        {
            invoked = true;
            s.AddSingleton(typeof(ILoggerAdapter<>), typeof(CustomLoggerAdapter<>));
        });

        Assert.True(invoked);
    }

    [Fact]
    public void ResolvedAdapter_UsesCategoryOfRequestedType()
    {
        var services = new ServiceCollection();
        services.AddLoggerAdapter();
        services.AddLogging();

        using var provider = services.BuildServiceProvider();
        var adapter = provider.GetRequiredService<ILoggerAdapter<SampleService>>();

        Assert.Equal(typeof(SampleService).FullName, adapter.CategoryName);
    }

    [Fact]
    public void AddLoggerAdapter_ReturnsSameServices()
    {
        var services = new ServiceCollection();

        Assert.Same(services, services.AddLoggerAdapter());
    }

    private sealed class CustomLoggerAdapter<TCategoryName> : ILoggerAdapter<TCategoryName>
    {
        public string CategoryName => "custom";
        public bool IsEnabled(LogLevel logLevel) => false;
        public void LogInformation(string message, params object?[] args) { }
        public void LogWarning(string message, params object?[] args) { }
        public void LogError(string message, params object?[] args) { }
        public void LogError(Exception exception, string message, params object?[] args) { }
        public void LogDebug(string message, params object?[] args) { }
        public void LogTrace(string message, params object?[] args) { }
        public void LogCritical(string message, params object?[] args) { }
        public void Log(LogLevel logLevel, string message, params object?[] args) { }
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
    }
}
