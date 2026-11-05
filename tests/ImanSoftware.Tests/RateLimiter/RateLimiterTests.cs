using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ImanSoftware.RateLimiter;
using ImanSoftware.RateLimiter.Abstractions;
using ImanSoftware.RateLimiter.Core.Enums;
using ImanSoftware.RateLimiter.Core.Models;
using ImanSoftware.RateLimiter.Infrastructure;
using ImanSoftware.RateLimiter.Middlewares;

namespace ImanSoftware.Tests.RateLimiter;

public class RateLimiterModelTests
{
    [Fact]
    public void RateLimitPolicy_Defaults_HaveExpectedValues()
    {
        var policy = new RateLimitPolicy();

        Assert.Equal(100, policy.PermitLimit);
        Assert.Equal(TimeSpan.FromMinutes(1), policy.Window);
        Assert.Equal(RateLimiterStrategy.FixedWindow, policy.Strategy);
        Assert.Equal(RateLimitPartitionBy.IpAddress, policy.PartitionBy);
        Assert.Null(policy.CustomPartitionResolver);
        Assert.True(policy.IncludeHeaders);
        Assert.Null(policy.QueueLimit);
        Assert.Null(policy.TokensPerSecond);
    }

    [Fact]
    public void RateLimiterOptions_Defaults_HaveExpectedValues()
    {
        var options = new ImanRateLimiterOptions();

        Assert.NotNull(options.DefaultPolicy);
        Assert.Empty(options.Policies);
        Assert.True(options.EnableMetrics);
        Assert.True(options.EnableLogging);
        Assert.Null(options.LogHandler);
        Assert.Null(options.OnRejected);
    }

    [Fact]
    public async Task RateLimitHeaders_ApplyTo_WritesStandardHeaders()
    {
        var context = new DefaultHttpContext();
        var reset = DateTimeOffset.FromUnixTimeSeconds(1700000000);

        new RateLimitHeaders { Limit = 100, Remaining = 42, Reset = reset }.ApplyTo(context.Response);

        Assert.Equal("100", context.Response.Headers["X-RateLimit-Limit"]);
        Assert.Equal("42", context.Response.Headers["X-RateLimit-Remaining"]);
        Assert.Equal("1700000000", context.Response.Headers["X-RateLimit-Reset"]);
        await Task.CompletedTask;
    }
}

public class DefaultRateLimiterPolicyTests
{
    private static DefaultHttpContext CreateContext(
        IPAddress? remoteIp = null,
        string? subClaim = null,
        string? nameIdClaim = null,
        string? apiKey = null,
        string path = "/api/values")
    {
        var context = new DefaultHttpContext();
        context.Connection.RemoteIpAddress = remoteIp ?? IPAddress.Parse("10.0.0.1");
        context.Request.Path = path;
        if (subClaim is not null)
        {
            context.User.AddIdentity(new ClaimsIdentity([new Claim("sub", subClaim)]));
        }

        if (nameIdClaim is not null)
        {
            context.User.AddIdentity(new ClaimsIdentity([new Claim("nameid", nameIdClaim)]));
        }

        if (apiKey is not null)
        {
            context.Request.Headers["X-API-Key"] = apiKey;
        }

        return context;
    }

    private static DefaultRateLimiterPolicy CreatePolicy(RateLimitPartitionBy partitionBy) =>
        new(new ImanRateLimiterOptions
        {
            DefaultPolicy = new RateLimitPolicy { PartitionBy = partitionBy }
        });

    [Fact]
    public void GetPartitionKey_IpAddress_UsesRemoteIpAddress()
    {
        Assert.Equal("10.0.0.1", CreatePolicy(RateLimitPartitionBy.IpAddress).GetPartitionKey(CreateContext()));
    }

    [Fact]
    public void GetPartitionKey_IpAddress_WithoutRemoteIp_ReturnsUnknown()
    {
        var context = new DefaultHttpContext();

        Assert.Equal("unknown", CreatePolicy(RateLimitPartitionBy.IpAddress).GetPartitionKey(context));
    }

    [Fact]
    public void GetPartitionKey_UserId_PrefersSubClaim()
    {
        var context = CreateContext(subClaim: "user-1", nameIdClaim: "user-2");

        Assert.Equal("user-1", CreatePolicy(RateLimitPartitionBy.UserId).GetPartitionKey(context));
    }

    [Fact]
    public void GetPartitionKey_UserId_WithoutSubClaim_ReturnsAnonymous()
    {
        Assert.Equal("anonymous", CreatePolicy(RateLimitPartitionBy.UserId).GetPartitionKey(CreateContext()));
    }

    [Fact]
    public void GetPartitionKey_ApiKey_ReadsXApiKeyHeader()
    {
        var context = CreateContext(apiKey: "key-123");

        Assert.Equal("key-123", CreatePolicy(RateLimitPartitionBy.ApiKey).GetPartitionKey(context));
    }

    [Fact]
    public void GetPartitionKey_ApiKey_WithoutHeader_ReturnsUnknown()
    {
        Assert.Equal("unknown", CreatePolicy(RateLimitPartitionBy.ApiKey).GetPartitionKey(CreateContext()));
    }

    [Fact]
    public void GetPartitionKey_Endpoint_UsesRequestPath()
    {
        var context = CreateContext(path: "/api/orders/5");

        Assert.Equal("/api/orders/5", CreatePolicy(RateLimitPartitionBy.Endpoint).GetPartitionKey(context));
    }

    [Fact]
    public void GetPartitionKey_Custom_UsesResolver()
    {
        var options = new ImanRateLimiterOptions
        {
            DefaultPolicy = new RateLimitPolicy
            {
                PartitionBy = RateLimitPartitionBy.Custom,
                CustomPartitionResolver = _ => "tenant-9"
            }
        };

        Assert.Equal("tenant-9", new DefaultRateLimiterPolicy(options).GetPartitionKey(CreateContext()));
    }

    [Fact]
    public void GetPartitionKey_Custom_WithoutResolver_ReturnsCustomFallback()
    {
        Assert.Equal("custom", CreatePolicy(RateLimitPartitionBy.Custom).GetPartitionKey(CreateContext()));
    }

    [Fact]
    public void GetPolicy_AlwaysReturnsDefaultPolicy()
    {
        var options = new ImanRateLimiterOptions
        {
            DefaultPolicy = new RateLimitPolicy { PermitLimit = 7 }
        };

        Assert.Equal(7, new DefaultRateLimiterPolicy(options).GetPolicy(new DefaultHttpContext()).PermitLimit);
    }
}

public class RateLimiterMetricsTests
{
    [Fact]
    public void RecordAllowed_IncrementsAllowedCounter()
    {
        var metrics = new RateLimiterMetrics();

        metrics.RecordAllowed("login", "10.0.0.1");
        metrics.RecordAllowed("login", "10.0.0.1");

        Assert.Equal((2, 0), metrics.GetMetrics("login", "10.0.0.1"));
    }

    [Fact]
    public void RecordRejected_IncrementsRejectedCounter()
    {
        var metrics = new RateLimiterMetrics();

        metrics.RecordRejected("login", "10.0.0.1", 30);

        Assert.Equal((0, 1), metrics.GetMetrics("login", "10.0.0.1"));
    }

    [Fact]
    public void GetMetrics_UnknownCombination_ReturnsZeroes()
    {
        Assert.Equal((0, 0), new RateLimiterMetrics().GetMetrics("nope", "nope"));
    }

    [Fact]
    public void Counters_AreIsolatedPerPolicyAndKey()
    {
        var metrics = new RateLimiterMetrics();
        metrics.RecordAllowed("login", "key-a");
        metrics.RecordRejected("api", "key-a", 30);

        Assert.Equal((1, 0), metrics.GetMetrics("login", "key-a"));
        Assert.Equal((0, 1), metrics.GetMetrics("api", "key-a"));
    }
}

public class RateLimiterRegistrationTests
{
    [Fact]
    public void AddImanRateLimiter_RegistersOptionsPolicyAndMetrics()
    {
        var services = new ServiceCollection();

        services.AddImanRateLimiter(o => o.DefaultPolicy.PermitLimit = 5);

        Assert.Contains(services, d => d.ServiceType == typeof(ImanRateLimiterOptions));
        Assert.Contains(services, d => d.ServiceType == typeof(IRateLimiterPolicy));
        Assert.Contains(services, d => d.ServiceType == typeof(IRateLimiterMetrics));

        using var provider = services.BuildServiceProvider();
        Assert.NotNull(provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<RateLimiterOptions>>());
    }

    [Fact]
    public void AddImanRateLimiter_ConfiguredOptionsAreResolvedAsSingleton()
    {
        var services = new ServiceCollection();

        services.AddImanRateLimiter(o =>
        {
            o.DefaultPolicy.PermitLimit = 5;
            o.Policies["login"] = new RateLimitPolicy { PermitLimit = 2 };
        });

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<ImanRateLimiterOptions>();
        Assert.Same(options, provider.GetRequiredService<ImanRateLimiterOptions>());
        Assert.Equal(5, options.DefaultPolicy.PermitLimit);
        Assert.Equal(2, options.Policies["login"].PermitLimit);
    }

    [Fact]
    public void AddImanRateLimiter_WithMetricsDisabled_DoesNotRegisterMetrics()
    {
        var services = new ServiceCollection();

        services.AddImanRateLimiter(o => o.EnableMetrics = false);

        Assert.DoesNotContain(services, d => d.ServiceType == typeof(IRateLimiterMetrics));
    }

    [Fact]
    public void AddImanRateLimiter_WithNullConfigure_ThrowsArgumentNullException()
    {
        var services = new ServiceCollection();

        Assert.Throws<ArgumentNullException>(
            () => services.AddImanRateLimiter((Action<ImanRateLimiterOptions>)null!));
    }

    [Fact]
    public void AddImanRateLimiter_FromConfiguration_BindsPolicies()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["DefaultPolicy:PermitLimit"] = "7",
                ["Policies:login:PermitLimit"] = "3",
                ["EnableMetrics"] = "false"
            })
            .Build();

        services.AddImanRateLimiter(configuration);

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<ImanRateLimiterOptions>();
        Assert.Equal(7, options.DefaultPolicy.PermitLimit);
        Assert.Equal(3, options.Policies["login"].PermitLimit);
        Assert.False(options.EnableMetrics);
        Assert.DoesNotContain(services, d => d.ServiceType == typeof(IRateLimiterMetrics));
    }
}

public class RateLimiterMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_CopiesResponseThroughBuffer()
    {
        var context = new DefaultHttpContext();
        var originalBody = new MemoryStream();
        context.Response.Body = originalBody;

        var middleware = new RateLimiterMiddleware(
            ctx =>
            {
                ctx.Response.StatusCode = 200;
                return ctx.Response.WriteAsync("hello from next");
            },
            new ImanRateLimiterOptions());

        await middleware.InvokeAsync(context);

        originalBody.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(originalBody, leaveOpen: true);
        Assert.Equal("hello from next", await reader.ReadToEndAsync());
        Assert.Equal(200, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_PlaceholderHeadersExtractReturnsNull_NoRateLimitHeadersWritten()
    {
        var context = new DefaultHttpContext();
        var originalBody = new MemoryStream();
        context.Response.Body = originalBody;

        var middleware = new RateLimiterMiddleware(
            _ => Task.CompletedTask,
            new ImanRateLimiterOptions());

        await middleware.InvokeAsync(context);

        Assert.False(context.Response.Headers.ContainsKey("X-RateLimit-Limit"));
        Assert.False(context.Response.Headers.ContainsKey("X-RateLimit-Policy"));
    }
}
