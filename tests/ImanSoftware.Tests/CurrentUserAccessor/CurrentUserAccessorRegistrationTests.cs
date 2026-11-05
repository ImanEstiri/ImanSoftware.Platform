using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ImanSoftware.CurrentUserAccessor;

namespace ImanSoftware.Tests.CurrentUserAccessor;

public class CurrentUserAccessorRegistrationTests
{
    [Fact]
    public void AddCurrentUserAccessor_RegistersOpenGenericHttpContextUserAccessorAsSingleton()
    {
        var services = new ServiceCollection();

        services.AddCurrentUserAccessor();

        var descriptor = Assert.Single(services, d => d.ServiceType == typeof(ICurrentUserAccessor<>));
        Assert.Equal(typeof(HttpContextUserAccessor<>), descriptor.ImplementationType);
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [Fact]
    public void AddCurrentUserAccessor_RegistersHttpContextAccessor()
    {
        var services = new ServiceCollection();

        services.AddCurrentUserAccessor();

        Assert.Contains(services, d => d.ServiceType == typeof(IHttpContextAccessor));
    }

    [Fact]
    public void AddCurrentUserAccessor_TryAdd_PreservesExistingRegistration()
    {
        var services = new ServiceCollection();
        services.AddSingleton(typeof(ICurrentUserAccessor<>), typeof(CustomUserAccessor<>));

        services.AddCurrentUserAccessor();

        var descriptor = Assert.Single(services, d => d.ServiceType == typeof(ICurrentUserAccessor<>));
        Assert.Equal(typeof(CustomUserAccessor<>), descriptor.ImplementationType);
    }

    [Fact]
    public void AddCurrentUserAccessor_WithCustomRegistration_InvokesDelegate()
    {
        var services = new ServiceCollection();
        var invoked = false;

        services.AddCurrentUserAccessor(s =>
        {
            invoked = true;
            s.AddSingleton<ICurrentUserAccessor<int>, IntUserAccessor>();
        });

        Assert.True(invoked);
        Assert.Contains(services, d => d.ServiceType == typeof(ICurrentUserAccessor<int>));
        Assert.DoesNotContain(services, d => d.ServiceType == typeof(ICurrentUserAccessor<>));
    }

    [Fact]
    public void AddCurrentUserAccessor_ReturnsSameServices()
    {
        var services = new ServiceCollection();

        Assert.Same(services, services.AddCurrentUserAccessor());
    }

    [Fact]
    public void ResolvedAccessor_UsesHttpContext()
    {
        var services = new ServiceCollection();
        services.AddCurrentUserAccessor();

        using var provider = services.BuildServiceProvider();
        var accessor = provider.GetRequiredService<ICurrentUserAccessor<int>>();

        Assert.IsType<HttpContextUserAccessor<int>>(accessor);
    }

    private sealed class CustomUserAccessor<TUserId> : ICurrentUserAccessor<TUserId>
    {
        public TUserId? GetCurrentUserId() => default;
        public System.Security.Claims.ClaimsPrincipal? GetCurrentUser() => null;
    }

    private sealed class IntUserAccessor : ICurrentUserAccessor<int>
    {
        public int GetCurrentUserId() => 1;
        public System.Security.Claims.ClaimsPrincipal? GetCurrentUser() => null;
    }
}
