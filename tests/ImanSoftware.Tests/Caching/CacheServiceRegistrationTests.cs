using Microsoft.Extensions.DependencyInjection;
using ImanSoftware.Caching;
using ImanSoftware.Serialization;

namespace ImanSoftware.Tests.Caching;

public class CacheServiceRegistrationTests
{
    [Fact]
    public void AddCacheService_RegistersMemoryCacheServiceAsSingleton()
    {
        var services = new ServiceCollection();

        services.AddCacheService();

        var descriptor = Assert.Single(services, d => d.ServiceType == typeof(ICacheService));
        Assert.Equal(typeof(MemoryCacheService), descriptor.ImplementationType);
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [Fact]
    public void AddCacheService_ResolvesWorkingInstance()
    {
        var services = new ServiceCollection();

        services.AddCacheService();

        using var provider = services.BuildServiceProvider();
        var cache = provider.GetRequiredService<ICacheService>();
        cache.Set("key", "value");

        Assert.Equal("value", cache.Get<string>("key"));
    }

    [Fact]
    public void AddCacheService_RegistersMemoryCacheAndSystemTextSerializerAsSideEffects()
    {
        var services = new ServiceCollection();

        services.AddCacheService();

        Assert.Contains(services, d => d.ServiceType == typeof(Microsoft.Extensions.Caching.Memory.IMemoryCache));
        Assert.Contains(services, d => d.ServiceType == typeof(IImanJsonSerializer));
    }

    [Fact]
    public void AddCacheService_TryAdd_PreservesExistingRegistration()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ICacheService, CustomCacheService>();

        services.AddCacheService();

        var descriptor = Assert.Single(services, d => d.ServiceType == typeof(ICacheService));
        Assert.Equal(typeof(CustomCacheService), descriptor.ImplementationType);
    }

    [Fact]
    public void AddCacheService_WithCustomImplementation_UsesAddSemantics()
    {
        var services = new ServiceCollection();

        services.AddCacheService(typeof(CustomCacheService));

        var descriptor = services.Single(d => d.ServiceType == typeof(ICacheService));
        Assert.Equal(typeof(CustomCacheService), descriptor.ImplementationType);
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [Fact]
    public void AddCacheService_CalledTwice_KeepsFirstDefaultRegistration()
    {
        var services = new ServiceCollection();

        services.AddCacheService();
        services.AddCacheService();

        Assert.Single(services, d => d.ServiceType == typeof(ICacheService));
    }

    private sealed class CustomCacheService : ICacheService
    {
        public T? Get<T>(string key) => default;
        public void Set<T>(string key, T value, TimeSpan? expiration = null) { }
        public void Remove(string key) { }
        public bool Exists(string key) => false;
        public T GetOrSet<T>(string key, Func<T> factory, TimeSpan? expiration = null) => factory();
    }
}
