using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ImanSoftware.Jwt;

namespace ImanSoftware.Tests.Jwt;

public class JwtTokenServiceRegistrationTests
{
    private static JwtOptions CreateOptions() => new()
    {
        SecretKey = "test-secret-key-for-imansoftware-jwt-tests-must-be-at-least-64-bytes-long!!",
        Issuer = "issuer",
        Audience = "audience",
        ExpiryMinutes = 30,
        ValidateLifetime = true
    };

    [Fact]
    public void AddJwtTokenService_WithNullOptionsAndNoCustomType_ThrowsArgumentNullException()
    {
        var services = new ServiceCollection();

        var exception = Assert.Throws<ArgumentNullException>(() => services.AddJwtTokenService(null));

        Assert.Equal("configureOptions", exception.ParamName);
    }

    [Fact]
    public void AddJwtTokenService_RegistersJwtTokenServiceAsSingleton()
    {
        var services = new ServiceCollection();

        services.AddJwtTokenService(CreateOptions());

        var descriptor = Assert.Single(services, d => d.ServiceType == typeof(IJwtTokenService));
        Assert.Equal(typeof(JwtTokenService), descriptor.ImplementationType);
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [Fact]
    public void AddJwtTokenService_CopiesOptionsIntoContainer()
    {
        var services = new ServiceCollection();
        var source = CreateOptions();

        services.AddJwtTokenService(source);

        using var provider = services.BuildServiceProvider();
        var resolved = provider.GetRequiredService<IOptions<JwtOptions>>().Value;

        Assert.Equal(source.SecretKey, resolved.SecretKey);
        Assert.Equal(source.Issuer, resolved.Issuer);
        Assert.Equal(source.Audience, resolved.Audience);
        Assert.Equal(source.ExpiryMinutes, resolved.ExpiryMinutes);
        Assert.Equal(source.ValidateLifetime, resolved.ValidateLifetime);
    }

    [Fact]
    public void AddJwtTokenService_ResolvesWorkingService()
    {
        var services = new ServiceCollection();

        services.AddJwtTokenService(CreateOptions());

        using var provider = services.BuildServiceProvider();
        var service = provider.GetRequiredService<IJwtTokenService>();
        var token = service.GenerateToken([new System.Security.Claims.Claim("sub", "1")]);

        Assert.True(service.ValidateToken(token).IsValid);
    }

    [Fact]
    public void AddJwtTokenService_WithCustomImplementation_RegistersCustomTypeWithoutOptions()
    {
        var services = new ServiceCollection();

        services.AddJwtTokenService(null, typeof(CustomTokenService));

        Assert.Contains(services, d => d.ServiceType == typeof(IJwtTokenService));
        Assert.DoesNotContain(services, d => d.ServiceType == typeof(IOptions<JwtOptions>));

        using var provider = services.BuildServiceProvider();
        Assert.IsType<CustomTokenService>(provider.GetRequiredService<IJwtTokenService>());
    }

    [Fact]
    public void AddJwtTokenService_WithCustomImplementation_IgnoresNullOptions()
    {
        var services = new ServiceCollection();

        // No exception thrown even though configureOptions is null.
        services.AddJwtTokenService(configureOptions: null, customImplementation: typeof(CustomTokenService));

        Assert.Contains(services, d => d.ServiceType == typeof(IJwtTokenService));
    }

    [Fact]
    public void AddJwtTokenService_ReturnsSameServices()
    {
        var services = new ServiceCollection();

        Assert.Same(services, services.AddJwtTokenService(CreateOptions()));
    }

    private sealed class CustomTokenService : IJwtTokenService
    {
        public string GenerateToken(IEnumerable<System.Security.Claims.Claim> claims) => "custom-token";
        public JwtTokenValidationResult ValidateToken(string token) => JwtTokenValidationResult.Failure("custom");
        public System.Security.Claims.ClaimsPrincipal? GetPrincipalFromExpiredToken(string token) => null;
    }
}
