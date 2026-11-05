using Microsoft.Extensions.DependencyInjection;
using ImanSoftware.DateTimeProvider;

namespace ImanSoftware.Tests.DateTimeProvider;

public class DefaultDateTimeProviderTests
{
    [Fact]
    public void Now_ReturnsLocalTimeWithinTolerance()
    {
        var before = DateTime.Now;

        var now = new DefaultDateTimeProvider().Now;

        var after = DateTime.Now;
        Assert.InRange(now.Ticks, before.AddSeconds(-5).Ticks, after.AddSeconds(5).Ticks);
    }

    [Fact]
    public void Today_ReturnsLocalDate()
    {
        Assert.Equal(DateOnly.FromDateTime(DateTime.Today), new DefaultDateTimeProvider().Today);
    }

    [Fact]
    public void CurrentTimeOfDay_ReturnsTimeWithinTolerance()
    {
        var before = TimeOnly.FromDateTime(DateTime.Now).AddMinutes(-1);
        var after = TimeOnly.FromDateTime(DateTime.Now).AddMinutes(1);

        var current = new DefaultDateTimeProvider().CurrentTimeOfDay;

        Assert.InRange(current.Ticks, before.Ticks, after.Ticks);
    }
}

public class DateTimeProviderRegistrationTests
{
    [Fact]
    public void AddDateTimeProvider_Default_RegistersDefaultImplementationAsSingleton()
    {
        var services = new ServiceCollection();

        services.AddDateTimeProvider();

        var descriptor = Assert.Single(services, d => d.ServiceType == typeof(IDateTimeProvider));
        Assert.Equal(typeof(DefaultDateTimeProvider), descriptor.ImplementationType);
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [Fact]
    public void AddDateTimeProvider_TryAdd_PreservesExistingRegistration()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IDateTimeProvider, FixedDateTimeProvider>();

        services.AddDateTimeProvider();

        var descriptor = Assert.Single(services, d => d.ServiceType == typeof(IDateTimeProvider));
        Assert.Equal(typeof(FixedDateTimeProvider), descriptor.ImplementationType);
    }

    [Fact]
    public void AddDateTimeProvider_WithCustomImplementation_UsesAddSemantics()
    {
        var services = new ServiceCollection();

        services.AddDateTimeProvider(typeof(FixedDateTimeProvider));

        var descriptor = Assert.Single(services, d => d.ServiceType == typeof(IDateTimeProvider));
        Assert.Equal(typeof(FixedDateTimeProvider), descriptor.ImplementationType);
    }

    [Fact]
    public void AddDateTimeProvider_ReturnsSameServices()
    {
        var services = new ServiceCollection();

        Assert.Same(services, services.AddDateTimeProvider());
    }

    private sealed class FixedDateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => new(2024, 1, 1, 10, 0, 0);
        public DateOnly Today => new(2024, 1, 1);
        public TimeOnly CurrentTimeOfDay => new(10, 0, 0);
    }
}
