using System.Net;
using Microsoft.Extensions.DependencyInjection;
using ImanSoftware.Branding;
using ImanSoftware.Branding.DependencyInjection;
using ImanSoftware.Branding.Infrastructure;
using ImanSoftware.Caching;

namespace ImanSoftware.Tests.Branding;

public class BrandingServiceTests
{
    private sealed class StubHttpHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;

        public List<HttpRequestMessage> Requests { get; } = new();

        public StubHttpHandler(Func<HttpRequestMessage, HttpResponseMessage> responder) => _responder = responder;

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            Requests.Add(request);

            // Honor cancellation deterministically (the service links a timeout CTS).
            if (cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException(cancellationToken);
            }

            return Task.FromResult(_responder(request));
        }
    }

    // BrandingService deserializes with default System.Text.Json options — property names
    // are case-sensitive and must match the BadgeData record parameter names exactly.
    private const string BadgeJson =
        """{"Developer":"Iman","Brand":"ImanSoftware","Website":"https://imansoftware.ir","Phone":"+98","Telegram":"iman_estiri","Email":"a@b.c","LinkedIn":"li","BadgeText":"Badge"}""";

    private static BrandingService CreateService(StubHttpHandler handler, Action<BrandingOptions>? configure = null)
    {
        var options = new BrandingOptions();
        configure?.Invoke(options);
        return new BrandingService(new HttpClient(handler), options);
    }

    [Fact]
    public async Task GetBadgeDataAsync_PrimarySuccess_ReturnsDataAndSkipsBackup()
    {
        var handler = new StubHttpHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(BadgeJson)
        });
        var service = CreateService(handler);

        var badge = await service.GetBadgeDataAsync("my-project");

        Assert.NotNull(badge);
        Assert.Equal("ImanSoftware", badge!.Brand);
        Assert.Equal(1, handler.Requests.Count);
        Assert.Contains("projectId=my-project", handler.Requests[0].RequestUri!.Query, StringComparison.Ordinal);
    }

    [Fact]
    public async Task GetBadgeDataAsync_PrimaryFails_FallsBackToBackup()
    {
        var handler = new StubHttpHandler(request =>
        {
            if (request.RequestUri!.ToString().Contains("backup", StringComparison.OrdinalIgnoreCase))
            {
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(BadgeJson) };
            }

            return new HttpResponseMessage(HttpStatusCode.InternalServerError);
        });
        var service = CreateService(handler, o => o.BackupApiUrl = "https://backup.example.com/badge");

        var badge = await service.GetBadgeDataAsync("my-project");

        Assert.NotNull(badge);
        Assert.Equal(2, handler.Requests.Count);
        Assert.StartsWith("https://backup.example.com/badge", handler.Requests[1].RequestUri!.ToString(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task GetBadgeDataAsync_BothApisFail_ReturnsNull()
    {
        var handler = new StubHttpHandler(_ => new HttpResponseMessage(HttpStatusCode.NotFound));
        var service = CreateService(handler);

        Assert.Null(await service.GetBadgeDataAsync("my-project"));
    }

    [Fact]
    public async Task GetBadgeDataAsync_PrimaryThrows_FallsBackToBackup()
    {
        var handler = new StubHttpHandler(request =>
        {
            if (request.RequestUri!.ToString().Contains("backup", StringComparison.OrdinalIgnoreCase))
            {
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(BadgeJson) };
            }

            throw new HttpRequestException("connection refused");
        });
        var service = CreateService(handler, o => o.BackupApiUrl = "https://backup.example.com/badge");

        var badge = await service.GetBadgeDataAsync("my-project");

        Assert.NotNull(badge);
        Assert.Equal(2, handler.Requests.Count);
    }

    [Fact]
    public async Task GetBadgeDataAsync_InvalidJsonFromPrimary_FallsBackToBackup()
    {
        var handler = new StubHttpHandler(request =>
        {
            if (request.RequestUri!.ToString().Contains("backup", StringComparison.OrdinalIgnoreCase))
            {
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(BadgeJson) };
            }

            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("not json") };
        });
        var service = CreateService(handler, o => o.BackupApiUrl = "https://backup.example.com/badge");

        var badge = await service.GetBadgeDataAsync("my-project");

        Assert.NotNull(badge);
    }

    [Fact]
    public async Task GetBadgeDataAsync_WithZeroTimeout_NeverThrows()
    {
        var handler = new StubHttpHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(BadgeJson)
        });
        // RequestTimeoutSeconds = 0 makes the primary attempt race with cancellation;
        // whichever way it resolves, the service must not throw.
        var service = CreateService(handler, o => o.RequestTimeoutSeconds = 0);

        var exception = await Record.ExceptionAsync(() => service.GetBadgeDataAsync("my-project"));

        Assert.Null(exception);
    }
}

public class BrandingCacheTests
{
    private sealed class RecordingCacheService : ICacheService
    {
        public object? LastSetValue { get; private set; }
        public TimeSpan? LastExpiration { get; private set; }
        public string? LastGetKey { get; private set; }
        private readonly Dictionary<string, object?> _store = new();

        public T? Get<T>(string key)
        {
            LastGetKey = key;
            return _store.TryGetValue(key, out var value) ? (T?)value : default;
        }

        public void Set<T>(string key, T value, TimeSpan? expiration = null)
        {
            _store[key] = value;
            LastSetValue = value;
            LastExpiration = expiration;
        }

        public void Remove(string key) => _store.Remove(key);
        public bool Exists(string key) => _store.ContainsKey(key);
        public T GetOrSet<T>(string key, Func<T> factory, TimeSpan? expiration = null) => factory();
    }

    [Fact]
    public async Task GetAsync_DelegatesToCacheService()
    {
        var cache = new RecordingCacheService();
        var expected = new BadgeData("d", "b", "w", "p", "t", "e", "l", "x");
        cache.Set("branding_p1", expected);

        var brandingCache = new BrandingCache(cache, new BrandingOptions());

        Assert.Equal(expected, await brandingCache.GetAsync("branding_p1"));
        Assert.Equal("branding_p1", cache.LastGetKey);
    }

    [Fact]
    public async Task SetAsync_UsesConfiguredCacheDuration()
    {
        var cache = new RecordingCacheService();
        var brandingCache = new BrandingCache(cache, new BrandingOptions { CacheDurationHours = TimeSpan.FromHours(3) });
        var value = new BadgeData("d", "b", "w", "p", "t", "e", "l", "x");

        await brandingCache.SetAsync("branding_p1", value);

        Assert.Equal(value, cache.LastSetValue);
        Assert.Equal(TimeSpan.FromHours(3), cache.LastExpiration);
    }
}

public class BrandingOptionsTests
{
    [Fact]
    public void Defaults_HaveExpectedValues()
    {
        var options = new BrandingOptions();

        Assert.Equal("https://imansoftware.ir/api/badge", options.PrimaryApiUrl);
        Assert.Equal(string.Empty, options.BackupApiUrl);
        Assert.Equal("Designed & Developed by ImanSoftware", options.DefaultBadgeText);
        Assert.Equal(TimeSpan.FromHours(12), options.CacheDurationHours);
        Assert.Equal(3, options.RequestTimeoutSeconds);
    }
}

public class BrandingRegistrationTests
{
    [Fact]
    public void AddImanBranding_RegistersServiceCacheAndOptions()
    {
        var services = new ServiceCollection();

        services.AddImanBranding();

        Assert.Contains(services, d => d.ServiceType == typeof(IBrandingService));
        Assert.Contains(services, d => d.ServiceType == typeof(IBrandingCache));
        Assert.Contains(services, d => d.ServiceType == typeof(BrandingOptions));
        Assert.Contains(services, d => d.ServiceType == typeof(ICacheService));
    }

    [Fact]
    public async Task AddImanBranding_ResolvesWorkingServiceAndCache()
    {
        var services = new ServiceCollection();

        services.AddImanBranding();

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        Assert.IsType<BrandingService>(scope.ServiceProvider.GetRequiredService<IBrandingService>());
        Assert.IsType<BrandingCache>(scope.ServiceProvider.GetRequiredService<IBrandingCache>());
        await Task.CompletedTask;
    }

    [Fact]
    public void AddImanBranding_AppliesConfigurationToSingletonOptions()
    {
        var services = new ServiceCollection();

        services.AddImanBranding(o =>
        {
            o.BackupApiUrl = "https://backup";
            o.RequestTimeoutSeconds = 9;
        });

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<BrandingOptions>();
        Assert.Equal("https://backup", options.BackupApiUrl);
        Assert.Equal(9, options.RequestTimeoutSeconds);
    }

    [Fact]
    public void AddImanBranding_SameOptionsInstanceIsShared()
    {
        var services = new ServiceCollection();

        services.AddImanBranding();

        using var provider = services.BuildServiceProvider();
        Assert.Same(
            provider.GetRequiredService<BrandingOptions>(),
            provider.GetRequiredService<BrandingOptions>());
    }
}
