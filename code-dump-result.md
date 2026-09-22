### nuget 

```xml 
<?xml version="1.0" encoding="utf-8"?>

<configuration>
	<packageSources>
		<clear />
		<!-- منبع اصلی: mirror داخلی که به nuget.org وصل می‌شود -->
		<!--<add key="LiaraMirror" value="https://package-mirror.liara.ir/repository/nuget/index.json" protocolVersion="3" />-->
		<!-- اگر mirror در دسترس نبود، می‌توانید این خط را به‌طور موقت با HTTP فعال کنید -->
		 <add key="nuget" value="https://api.nuget.org/v3/index.json" protocolVersion="3" /> 
	</packageSources>
	<packageSourceMapping>
		<packageSource key="nuget">
			<package pattern="*" />
		</packageSource>
	</packageSourceMapping>
</configuration>
``` 


### src > ImanSoftware.Branding > ImanSoftware.Branding 

```xml 
<Project Sdk="Microsoft.NET.Sdk.Razor">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <GeneratePackageOnBuild>true</GeneratePackageOnBuild>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="ImanSoftware.Caching" />
    <PackageReference Include="ImanSoftware.Logging" />
  </ItemGroup>
  <PropertyGroup>
    <PackageId>ImanSoftware.Branding</PackageId>
    <Version>1.0.12</Version>
    <Description>Display ImanSoftware branding badge with contact information in Blazor apps.</Description>
    <PackageTags>blazor;branding;badge;imansoftware</PackageTags>
  </PropertyGroup>
  <ItemGroup>
    <FrameworkReference Include="Microsoft.AspNetCore.App" />
  </ItemGroup>
</Project>
``` 


### src > ImanSoftware.Branding > Abstractions > IBrandingCache 

```csharp 
﻿
namespace ImanSoftware.Branding;

public interface IBrandingCache
{
    Task<BadgeData?> GetAsync(string key);
    Task SetAsync(string key, BadgeData value);
}

``` 


### src > ImanSoftware.Branding > Abstractions > IBrandingService 

```csharp 
﻿
namespace ImanSoftware.Branding;

public interface IBrandingService
{
    Task<BadgeData?> GetBadgeDataAsync(string projectId, CancellationToken cancellationToken = default);
}
``` 


### src > ImanSoftware.Branding > Components > ImanSoftwareBadge 

```razor 
@namespace ImanSoftware.Branding
@inject IBrandingService BrandingService
@inject IBrandingCache Cache

@{
    var toggleId = $"imansoftware-toggle-{ProjectId ?? "default"}";
}

@if (!string.IsNullOrEmpty(_badgeText))
{
    <input type="checkbox" id="@toggleId" class="imansoftware-modal-toggle" />
    <label for="@toggleId" class="imansoftware-badge">
        <span>@_badgeText</span>
        <span class="badge-dot">⚡</span>
    </label>

    <div class="imansoftware-modal-overlay">
        <label for="@toggleId" class="imansoftware-modal-backdrop"></label>
        <div class="imansoftware-modal-content">
            <div class="imansoftware-modal-avatar">
                <span>⚡</span>
            </div>
            <h3>@_data.Brand</h3>
            <p class="modal-subtitle">توسعه‌دهنده: <strong>@_data.Developer</strong></p>

            <div class="modal-links">
                @if (!string.IsNullOrEmpty(_data.Website))
                {
                    <a href="@_data.Website" target="_blank" class="modal-link">
                        <span>🌐</span> وبسایت
                    </a>
                }
                @if (!string.IsNullOrEmpty(_data.Telegram))
                {
                    <a href="https://t.me/@_data.Telegram" target="_blank" class="modal-link">
                        <span>✈️</span> تلگرام
                    </a>
                }
                @if (!string.IsNullOrEmpty(_data.Email))
                {
                    <a href="mailto:@_data.Email" class="modal-link">
                        <span>📧</span> ایمیل
                    </a>
                }
                @if (!string.IsNullOrEmpty(_data.Phone))
                {
                    <a href="tel:@_data.Phone" class="modal-link">
                        <span>📞</span> تماس
                    </a>
                }
                @if (!string.IsNullOrEmpty(_data.LinkedIn))
                {
                    <a href="@_data.LinkedIn" target="_blank" class="modal-link">
                        <span>💼</span> لینکدین
                    </a>
                }
            </div>

            <label for="@toggleId" class="modal-close-btn">بستن</label>
        </div>
    </div>
}

<style>
    /* ========== Badge ========== */
    .imansoftware-badge {
        display: inline-flex;
        align-items: center;
        gap: 6px;
        cursor: pointer;
        padding: 6px 14px;
        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
        color: #fff;
        border-radius: 20px;
        font-size: 13px;
        font-weight: 500;
        font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
        transition: all 0.3s ease;
        user-select: none;
        box-shadow: 0 2px 8px rgba(102, 126, 234, 0.3);
        text-decoration: none;
        margin: 0;
    }

        .imansoftware-badge:hover {
            transform: translateY(-2px);
            box-shadow: 0 4px 12px rgba(102, 126, 234, 0.5);
            color: #fff;
            text-decoration: none;
        }

        .imansoftware-badge .badge-dot {
            font-size: 14px;
            animation: iman-pulse 2s infinite;
        }

    @@keyframes iman-pulse {
        0%, 100% {
            opacity: 1;
        }

        50% {
            opacity: 0.6;
        }
    }

    /* ========== Checkbox (مخفی) ========== */
    .imansoftware-modal-toggle {
        display: none !important;
        position: absolute;
        opacity: 0;
        pointer-events: none;
    }

    /* ================================================================
           Modal Overlay
           - همیشه display: flex است تا transitionها کار کنند
           - با visibility + opacity مخفی/نمایان می‌شود
           - هنگام بستن، visibility با تأخیر ۰.۴s غیرفعال می‌شود
           ================================================================ */
    .imansoftware-modal-overlay {
        display: flex;
        position: fixed !important;
        top: 0 !important;
        left: 0 !important;
        right: 0 !important;
        bottom: 0 !important;
        width: 100vw !important;
        height: 100vh !important;
        z-index: 2147483647 !important;
        align-items: center;
        justify-content: center;
        padding: 20px;
        box-sizing: border-box;
        /* حالت مخفی */
        visibility: hidden;
        opacity: 0;
        pointer-events: none;
        /* انیمیشن بستن: opacity سریع، visibility با تأخیر */
        transition: opacity 0.3s cubic-bezier(0.4, 0, 0.2, 1), visibility 0s linear 0.4s;
    }

    .imansoftware-modal-toggle:checked ~ .imansoftware-modal-overlay {
        visibility: visible;
        opacity: 1;
        pointer-events: auto;
        /* انیمیشن باز شدن: visibility فوری */
        transition: opacity 0.3s cubic-bezier(0.4, 0, 0.2, 1), visibility 0s linear 0s;
    }

    /* ================================================================
           Backdrop
           - بلور و رنگ پس‌زمینه به صورت نرم transition می‌شوند
           ================================================================ */
    .imansoftware-modal-backdrop {
        position: absolute;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        text-decoration: none;
        cursor: pointer;
        will-change: backdrop-filter, background-color;
        /* حالت اولیه: کاملاً شفاف */
        background: rgba(15, 15, 30, 0);
        backdrop-filter: blur(0px) saturate(100%);
        -webkit-backdrop-filter: blur(0px) saturate(100%);
        transition: backdrop-filter 0.45s cubic-bezier(0.4, 0, 0.2, 1), -webkit-backdrop-filter 0.45s cubic-bezier(0.4, 0, 0.2, 1), background-color 0.45s cubic-bezier(0.4, 0, 0.2, 1);
    }

    .imansoftware-modal-toggle:checked ~ .imansoftware-modal-overlay .imansoftware-modal-backdrop {
        background: rgb(15 15 30 / 75%);
        backdrop-filter: blur(2px) saturate(180%);
        -webkit-backdrop-filter: blur(2px) saturate(180%);
    }

    /* ================================================================
           Modal Content
           - با transform + opacity به صورت نرم ظاهر/ناپدید می‌شود
           ================================================================ */
    .imansoftware-modal-content {
        position: relative;
        background: rgba(255, 255, 255, 0.98);
        padding: 32px 36px 28px;
        border-radius: 20px;
        max-width: 420px;
        width: 100%;
        box-shadow: 0 25px 70px rgba(0, 0, 0, 0.35), 0 0 0 1px rgba(255, 255, 255, 0.5) inset;
        text-align: center;
        font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
        direction: rtl;
        z-index: 1;
        box-sizing: border-box;
        /* حالت اولیه: کوچک و پایین‌تر */
        transform: scale(0.92) translateY(20px);
        opacity: 0;
        transition: transform 0.4s cubic-bezier(0.16, 1, 0.3, 1), opacity 0.3s cubic-bezier(0.4, 0, 0.2, 1);
    }

    .imansoftware-modal-toggle:checked ~ .imansoftware-modal-overlay .imansoftware-modal-content {
        transform: scale(1) translateY(0);
        opacity: 1;
        transition-delay: 0.05s;
    }

    /* ========== Avatar ========== */
    .imansoftware-modal-avatar {
        width: 64px;
        height: 64px;
        margin: -60px auto 16px;
        display: flex;
        align-items: center;
        justify-content: center;
        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
        border-radius: 50%;
        box-shadow: 0 8px 24px rgba(102, 126, 234, 0.5);
        border: 4px solid #fff;
        font-size: 28px;
    }

    .imansoftware-modal-content h3 {
        margin: 0 0 6px 0;
        color: #2d3748;
        font-size: 22px;
        font-weight: 700;
    }

    .imansoftware-modal-content .modal-subtitle {
        margin: 0 0 20px 0;
        color: #718096;
        font-size: 14px;
    }

        .imansoftware-modal-content .modal-subtitle strong {
            color: #667eea;
            font-weight: 600;
        }

    /* ========== Links Grid ========== */
    .imansoftware-modal-content .modal-links {
        display: grid;
        grid-template-columns: repeat(2, 1fr);
        gap: 10px;
        margin-bottom: 20px;
    }

    .imansoftware-modal-content .modal-link {
        display: flex;
        align-items: center;
        justify-content: center;
        gap: 6px;
        padding: 10px 12px;
        color: #4a5568;
        background: #f7f8fc;
        text-decoration: none;
        border-radius: 10px;
        font-size: 13px;
        font-weight: 500;
        transition: all 0.2s ease;
        border: 1px solid transparent;
    }

        .imansoftware-modal-content .modal-link:hover {
            background: #667eea;
            color: #fff;
            transform: translateY(-2px);
            box-shadow: 0 6px 16px rgba(102, 126, 234, 0.35);
            text-decoration: none;
        }

        .imansoftware-modal-content .modal-link span {
            font-size: 15px;
        }

    /* ========== Close Button ========== */
    .imansoftware-modal-content .modal-close-btn {
        display: block;
        width: 100%;
        padding: 12px;
        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
        color: #fff;
        border: none;
        border-radius: 10px;
        font-size: 14px;
        font-weight: 600;
        cursor: pointer;
        transition: all 0.2s ease;
        font-family: inherit;
        text-decoration: none;
        box-sizing: border-box;
        text-align: center;
    }

        .imansoftware-modal-content .modal-close-btn:hover {
            transform: translateY(-2px);
            box-shadow: 0 8px 20px rgba(102, 126, 234, 0.5);
            color: #fff;
            text-decoration: none;
        }

    /* ========== Responsive ========== */
    @@media (max-width: 480px) {
        .imansoftware-modal-content {
            padding: 28px 22px 22px;
            max-width: 100%;
        }

            .imansoftware-modal-content h3 {
                font-size: 18px;
            }

            .imansoftware-modal-content .modal-links {
                grid-template-columns: 1fr;
            }

        .imansoftware-badge {
            font-size: 12px;
            padding: 5px 12px;
        }
    }
</style>

@code {
    [Parameter] public string ProjectId { get; set; } = "default";
    [Parameter] public string? CustomBadgeText { get; set; }

    private static readonly BadgeData DefaultBadge = new(
        Developer: "Iman Estiri",
        Brand: "ImanSoftware",
        Website: "https://imansoftware.ir",
        Phone: "+989330884150",
        Telegram: "iman_estiri",
        Email: "contact.imanestiri@gmail.com",
        LinkedIn: "https://ir.linkedin.com/in/javad-estiri-bb3598229",
        BadgeText: "Designed & Developed by ImanSoftware");

    private BadgeData _data = DefaultBadge;
    private string _badgeText = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        if (string.IsNullOrWhiteSpace(ProjectId))
        {
            _badgeText = CustomBadgeText ?? DefaultBadge.BadgeText;
            return;
        }

        var cacheKey = $"branding_{ProjectId}";
        var cached = await Cache.GetAsync(cacheKey);
        if (cached is not null)
        {
            _data = cached;
            _badgeText = _data.BadgeText ?? CustomBadgeText ?? DefaultBadge.BadgeText;
            return;
        }

        try
        {
            var result = await BrandingService.GetBadgeDataAsync(ProjectId);
            if (result is not null)
            {
                _data = result;
                await Cache.SetAsync(cacheKey, _data);
                _badgeText = _data.BadgeText ?? CustomBadgeText ?? DefaultBadge.BadgeText;
                return;
            }
        }
        catch { /* ignore */ }

        _data = DefaultBadge;
        _badgeText = CustomBadgeText ?? DefaultBadge.BadgeText;
    }
}
``` 


### src > ImanSoftware.Branding > Core > BadgeData 

```csharp 
﻿namespace ImanSoftware.Branding;

public record BadgeData(
    string Developer,
    string Brand,
    string Website,
    string Phone,
    string Telegram,
    string Email,
    string LinkedIn,
    string BadgeText);
``` 


### src > ImanSoftware.Branding > Core > BrandingOptions 

```csharp 
﻿namespace ImanSoftware.Branding;

public class BrandingOptions
{
    public string PrimaryApiUrl = "https://imansoftware.ir/api/badge";
    public string BackupApiUrl = ""; 
    public string DefaultBadgeText { get; set; } = "Designed & Developed by ImanSoftware";
    public TimeSpan CacheDurationHours { get; set; } = TimeSpan.FromHours(12);
    public int RequestTimeoutSeconds { get; set; } = 3;
}
``` 


### src > ImanSoftware.Branding > DependencyInjection > DependencyInjection 

```csharp 
﻿using ImanSoftware.Branding.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ImanSoftware.Caching;
using ImanSoftware.Logging;

namespace ImanSoftware.Branding.DependencyInjection;

public static class BrandingServiceCollectionExtensions
{
    public static IServiceCollection AddImanBranding(
        this IServiceCollection services,
        Action<BrandingOptions>? configure = null)
    {
        // 1. تنظیمات
        var options = new BrandingOptions();
        configure?.Invoke(options);
        services.AddSingleton(options);

        services.AddCacheService();
        services.AddLoggerAdapter();

        services.AddHttpClient<IBrandingService, BrandingService>();
        services.TryAddScoped<IBrandingCache, BrandingCache>();

        return services;
    }
}
``` 


### src > ImanSoftware.Branding > Infrastructure > BrandingCache 

```csharp 
﻿
using ImanSoftware.Caching; 

namespace ImanSoftware.Branding.Infrastructure;

internal class BrandingCache : IBrandingCache
{
    private readonly ICacheService _cache;
    private readonly BrandingOptions _options;

    public BrandingCache(ICacheService cache, BrandingOptions options)
    {
        _cache = cache;
        _options = options;
    }

    public async Task<BadgeData?> GetAsync(string key)
    {
        return await Task.FromResult(_cache.Get<BadgeData>(key));
    }

    public async Task SetAsync(string key, BadgeData value)
    {
        await Task.Run(() => _cache.Set(key, value, _options.CacheDurationHours));
    }
}
``` 


### src > ImanSoftware.Branding > Infrastructure > BrandingService 

```csharp 
﻿using System.Text.Json;
using ImanSoftware.Logging;

namespace ImanSoftware.Branding.Infrastructure;

internal class BrandingService : IBrandingService
{
    private readonly HttpClient _httpClient;
    private readonly BrandingOptions _options;
    private readonly ILoggerAdapter<BrandingService>? _logger;

    public BrandingService(HttpClient httpClient, BrandingOptions options, ILoggerAdapter<BrandingService>? logger = null)
    {
        _httpClient = httpClient;
        _options = options;
        _logger = logger;
    }

    public async Task<BadgeData?> GetBadgeDataAsync(string projectId, CancellationToken cancellationToken = default)
    {
        try
        {
            var primaryUrl = $"{_options.PrimaryApiUrl}?projectId={Uri.EscapeDataString(projectId)}";
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(_options.RequestTimeoutSeconds));

            var response = await _httpClient.GetAsync(primaryUrl, cts.Token);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync(cts.Token);
                var data = JsonSerializer.Deserialize<BadgeData>(json);
                if (data is not null)
                    return data;
            }
        }
        catch (Exception ex)
        {
            _logger?.LogWarning("Failed to fetch from Primary API. Error : {ex} . Falling back to Backup." , ex);
        }

        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(_options.RequestTimeoutSeconds));

            var response = await _httpClient.GetAsync(_options.BackupApiUrl, cts.Token);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync(cts.Token);
                var data = JsonSerializer.Deserialize<BadgeData>(json);
                if (data is not null)
                    return data;
            }
        }
        catch (Exception ex)
        {
            _logger?.LogWarning( "Failed to fetch from Backup API. Error : {ex}",ex);
        }

        return null;
    }
}
``` 


### src > ImanSoftware.Caching > ImanSoftware.Caching 

```xml 
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.Caching.Abstractions" />
    <PackageReference Include="Microsoft.Extensions.Caching.Memory" />
  </ItemGroup>
  <ItemGroup>
    <PackageReference Include="ImanSoftware.Serialization" />
  </ItemGroup>
  <PropertyGroup>
    <PackageId>ImanSoftware.Caching</PackageId>
    <Version>1.0.2</Version>
    <Description>In-memory caching abstraction built on IMemoryCache, with a clean GetOrSet pattern.</Description>
    <PackageTags>caching;memory;cache;abstraction;in-memory</PackageTags>
  </PropertyGroup>
</Project>
``` 


### src > ImanSoftware.Caching > Abstractions > CacheServiceRegistration 

```csharp 
﻿using ImanSoftware.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ImanSoftware.Caching;

public static class CacheServiceRegistration
{
    public static IServiceCollection AddCacheService(
        this IServiceCollection services,
        Type? customImplementation = null)
    {
        services.AddImanSerialization(ImplementationType: SerializationDefaultImplementationType.SystemText);
        services.AddMemoryCache();
        if (customImplementation is not null)
        {
            services.AddSingleton(typeof(ICacheService), customImplementation);
        }
        else 
        {
            services.TryAddSingleton<ICacheService, MemoryCacheService>();
        }

        return services;
    }
}
``` 


### src > ImanSoftware.Caching > Abstractions > ICacheService 

```csharp 
﻿
namespace ImanSoftware.Caching;

/// <summary>
/// Contract for caching data with support for different providers (Memory, Redis, etc.).
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Gets a cached value by key.
    /// </summary>
    T? Get<T>(string key);

    /// <summary>
    /// Sets a value in the cache with an optional expiration time.
    /// </summary>
    void Set<T>(string key, T value, TimeSpan? expiration = null);

    /// <summary>
    /// Removes a value from the cache by key.
    /// </summary>
    void Remove(string key);

    /// <summary>
    /// Checks if a key exists in the cache.
    /// </summary>
    bool Exists(string key);

    /// <summary>
    /// Gets a value or sets it if not exists (atomic operation).
    /// </summary>
    T GetOrSet<T>(string key, Func<T> factory, TimeSpan? expiration = null);
}
``` 


### src > ImanSoftware.Caching > Infrastructure > MemoryCacheService 

```csharp 
﻿using ImanSoftware.Serialization;
using Microsoft.Extensions.Caching.Memory;

namespace ImanSoftware.Caching;

/// <summary>
/// Implementation of ICacheService using IMemoryCache (in-memory caching).
/// Uses IJsonSerializer to store objects as JSON strings.
/// </summary>
internal class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IImanJsonSerializer _jsonSerializer;

    public MemoryCacheService(IMemoryCache memoryCache, IImanJsonSerializer jsonSerializer)
    {
        _memoryCache = memoryCache;
        _jsonSerializer = jsonSerializer;
    }

    public T? Get<T>(string key)
    {
        var json = _memoryCache.Get<string>(key);
        if (json is null)
            return default;

        return _jsonSerializer.Deserialize<T>(json);
    }

    public void Set<T>(string key, T value, TimeSpan? expiration = null)
    {
        var json = _jsonSerializer.Serialize(value);
        var options = new MemoryCacheEntryOptions();

        if (expiration.HasValue)
            options.AbsoluteExpirationRelativeToNow = expiration.Value;

        _memoryCache.Set(key, json, options);
    }

    public void Remove(string key)
    {
        _memoryCache.Remove(key);
    }

    public bool Exists(string key)
    {
        return _memoryCache.TryGetValue(key, out _);
    }

    public T GetOrSet<T>(string key, Func<T> factory, TimeSpan? expiration = null)
    {
        // استفاده از GetOrCreate برای جلوگیری از Race Condition
        var json = _memoryCache.GetOrCreate<string>(key, entry =>
        {
            if (expiration.HasValue)
                entry.AbsoluteExpirationRelativeToNow = expiration.Value;

            var value = factory();
            return _jsonSerializer.Serialize(value);
        });

        // اگر json null برگردد (که نباید اینطور باشد)، مقدار را دوباره تولید می‌کنیم
        if (json is null)
            return factory();

        return _jsonSerializer.Deserialize<T>(json);
    }
}
``` 


### src > ImanSoftware.CQRS > ImanSoftware.CQRS 

```xml 
﻿<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="MediatR" />
  </ItemGroup>



  <PropertyGroup>
    <PackageId>ImanSoftware.CQRS</PackageId>
    <Version>1.0.0</Version>
    <Description>A lightweight CQRS abstraction layer built on MediatR for .NET applications.Provides base interfaces (ICommand, IQuery, ICommandHandler, IQueryHandler) to implement the Command Query Responsibility Segregation pattern with minimal boilerplate.</Description>
    <PackageTags>cqrs;mediatr;command;query;handler;mediator;clean-architecture;ddd;dotnet;aspnetcore</PackageTags>
  </PropertyGroup>


</Project>

``` 


### src > ImanSoftware.CQRS > Abstractions > ICommand 

```csharp 
﻿// مسیر: Abstractions/ICommand.cs
using MediatR;

namespace ImanSoftware.CQRS;

/// <summary>
/// Base interface for a command (without a response).
/// </summary>
public interface ICommand : IRequest
{
}

/// <summary>
/// Base interface for a command with a response.
/// </summary>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface ICommand<out TResponse> : IRequest<TResponse>
{
}
``` 


### src > ImanSoftware.CQRS > Abstractions > ICommandHandler 

```csharp 
﻿// مسیر: Abstractions/ICommandHandler.cs
using MediatR;

namespace ImanSoftware.CQRS;

/// <summary>
/// Base interface for a command handler (without response).
/// </summary>
public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand>
    where TCommand : ICommand
{
}

/// <summary>
/// Base interface for a command handler with a response.
/// </summary>
public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
}
``` 


### src > ImanSoftware.CQRS > Abstractions > IQuery 

```csharp 
﻿// مسیر: Abstractions/IQuery.cs
using MediatR;

namespace ImanSoftware.CQRS;

/// <summary>
/// Base interface for a query.
/// </summary>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface IQuery<out TResponse> : IRequest<TResponse>
{
}
``` 


### src > ImanSoftware.CQRS > Abstractions > IQueryHandler 

```csharp 
﻿// مسیر: Abstractions/IQueryHandler.cs
using MediatR;

namespace ImanSoftware.CQRS;

/// <summary>
/// Base interface for a query handler.
/// </summary>
public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
}
``` 


### src > ImanSoftware.CQRS > DependencyInjection > CqrsServiceCollectionExtensions 

```csharp 
﻿// مسیر: DependencyInjection/CqrsServiceCollectionExtensions.cs
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ImanSoftware.CQRS.DependencyInjection;

public static class CqrsServiceCollectionExtensions
{
    /// <summary>
    /// Adds CQRS (MediatR) services to the DI container.
    /// </summary>
    public static IServiceCollection AddCqrs(
        this IServiceCollection services,
        Assembly[] assemblies,
        Action<MediatRServiceConfiguration>? configure = null)
    {
        var configuration = new MediatRServiceConfiguration();
        configuration.RegisterServicesFromAssemblies(assemblies);

        configure?.Invoke(configuration);

        services.AddMediatR(configuration);

        return services;
    }
}
``` 


### src > ImanSoftware.CQRS > Infrastructure > MediatorExtensions 

```csharp 
﻿// مسیر: Infrastructure/MediatorExtensions.cs
using MediatR;

namespace ImanSoftware.CQRS;

public static class MediatorExtensions
{
    /// <summary>
    /// Sends a command without a response.
    /// </summary>
    public static async Task SendCommandAsync<TCommand>(
        this IMediator mediator,
        TCommand command,
        CancellationToken cancellationToken = default)
        where TCommand : ICommand
    {
        await mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Sends a command and returns a response.
    /// </summary>
    public static async Task<TResponse> SendCommandAsync<TCommand, TResponse>(
        this IMediator mediator,
        TCommand command,
        CancellationToken cancellationToken = default)
        where TCommand : ICommand<TResponse>
    {
        return await mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Sends a query and returns a response.
    /// </summary>
    public static async Task<TResponse> SendQueryAsync<TQuery, TResponse>(
        this IMediator mediator,
        TQuery query,
        CancellationToken cancellationToken = default)
        where TQuery : IQuery<TResponse>
    {
        return await mediator.Send(query, cancellationToken);
    }
}
``` 


### src > ImanSoftware.CurrentUserAccessor > ImanSoftware.CurrentUserAccessor 

```xml 
﻿<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Http" />
  </ItemGroup>


  <PropertyGroup>
    <PackageId>ImanSoftware.CurrentUserAccessor</PackageId>
    <Version>1.0.0</Version>
    <Description>Access the current authenticated user via HttpContext with generic TUserId support (Guid, int, string).</Description>
    <PackageTags>user;claims;authentication;httpcontext;currentuser</PackageTags>
  </PropertyGroup>


</Project>

``` 


### src > ImanSoftware.CurrentUserAccessor > Abstractions > CurrentUserAccessorRegistration 

```csharp 
﻿
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ImanSoftware.CurrentUserAccessor;

public static class CurrentUserAccessorRegistration
{

    public static IServiceCollection AddCurrentUserAccessor(
        this IServiceCollection services, Action<IServiceCollection>? customImplementation = null)
    {
        services.AddHttpContextAccessor();
        if (customImplementation is not null)
        {
            customImplementation(services);
        }
        else
        {
            services.TryAddSingleton(typeof(ICurrentUserAccessor<>), typeof(HttpContextUserAccessor<>));
        }

        return services;
    }

}

``` 


### src > ImanSoftware.CurrentUserAccessor > Abstractions > ICurrentUserAccessor 

```csharp 
﻿using System.Security.Claims;

namespace ImanSoftware.CurrentUserAccessor;

/// <summary>
/// Contract for accessing the currently authenticated user's information.
/// Generic type TUserId allows flexibility for different ID types (e.g., int, Guid, string).
/// </summary>
public interface ICurrentUserAccessor<out TUserId>
{
    /// <summary>
    /// Retrieves the unique identifier of the currently logged-in user.
    /// </summary>
    TUserId? GetCurrentUserId();

    /// <summary>
    /// Retrieves the complete ClaimsPrincipal object representing the current user.
    /// </summary>
    ClaimsPrincipal? GetCurrentUser();
}
``` 


### src > ImanSoftware.CurrentUserAccessor > Infrastructure > HttpContextUserAccessor 

```csharp 
﻿using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.Security.Claims;

namespace ImanSoftware.CurrentUserAccessor;

/// <summary>
/// HTTP-specific implementation of ICurrentUserAccessor.
/// Uses .NET's TypeDescriptor to automatically convert the 'sub' claim into TUserId.
/// Supports int, long, Guid, string, and other primitive types out-of-the-box.
/// </summary>
public class HttpContextUserAccessor<TUserId> : ICurrentUserAccessor<TUserId>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextUserAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Gets the current user's ID by reading the 'sub' claim and automatically converting it to TUserId.
    /// Returns default(TUserId) if conversion fails or user is not authenticated.
    /// </summary>
    public TUserId? GetCurrentUserId()
    {
        var user = GetCurrentUser();
        if (user is null)
            return default;

        var userIdClaim = user.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
            return default;

        try
        {
            var converter = TypeDescriptor.GetConverter(typeof(TUserId));
            if (converter.CanConvertFrom(typeof(string)))
            {
                return (TUserId?)converter.ConvertFromInvariantString(userIdClaim);
            }
            return default;
        }
        catch
        {
            return default;
        }
    }

    /// <summary>
    /// Gets the ClaimsPrincipal object from the current HttpContext.
    /// </summary>
    public ClaimsPrincipal? GetCurrentUser()
    {
        return _httpContextAccessor.HttpContext?.User;
    }
}
``` 


### src > ImanSoftware.DataAccess > ImanSoftware.DataAccess 

```xml 
﻿<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="ImanSoftware.Outcome" />
  </ItemGroup>

  <PropertyGroup>
    <PackageId>ImanSoftware.DataAccess</PackageId>
    <Version>1.0.0</Version>
    <Description>Database-agnostic data access abstractions (connection factory, transaction scope, query executor).</Description>
    <PackageTags>dataaccess;sql;abstraction;repository;database</PackageTags>
  </PropertyGroup>


</Project>

``` 


### src > ImanSoftware.DataAccess > Abstractions > DataAccessRegistration 

```csharp 
﻿using Microsoft.Extensions.DependencyInjection;

namespace ImanSoftware.DataAccess;

public static class DataAccessRegistration
{
    public static IServiceCollection AddDataAccess(
        this IServiceCollection services,
        Action<IServiceCollection> customRegistration)
    {
        ArgumentNullException.ThrowIfNull(customRegistration , paramName: "CustomRegistration for ImanSoftware.DataAccess");

        customRegistration(services);

        return services;
    }
}
``` 


### src > ImanSoftware.DataAccess > Abstractions > IDbConnectionFactory 

```csharp 
﻿// Abstractions/DataAccess/IDbConnectionFactory.cs
using System.Data;

namespace ImanSoftware.DataAccess;

/// <summary>
/// Factory for creating database connections.
/// </summary>
public interface IDbConnectionFactory
{
    /// <summary>
    /// Creates a new database connection.
    /// </summary>
    IDbConnection CreateConnection();

    /// <summary>
    /// Gets the connection string.
    /// </summary>
    string ConnectionString { get; }
}
``` 


### src > ImanSoftware.DataAccess > Abstractions > ISqlDataAccessor 

```csharp 
﻿// Abstractions/DataAccess/IRepository.cs

namespace ImanSoftware.DataAccess;

using ImanSoftware.Outcomes;

using System.Data;


/// <summary>
/// Contract for SQL data access operations.
/// Supports both stored procedures and ad-hoc queries.
/// </summary>
public interface ISqlDataAccessor
{
    /// <summary>
    /// Executes a query and returns a single result.
    /// </summary>
    Task<Outcome<T>> QuerySingleAsync<T>(
        string sql,
        object? parameters = null,
        CommandType commandType = CommandType.Text,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a query and returns a collection of results.
    /// </summary>
    Task<Outcome<IEnumerable<T>>> QueryAsync<T>(
        string sql,
        object? parameters = null,
        CommandType commandType = CommandType.Text,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a non-query command and returns the number of affected rows.
    /// </summary>
    Task<Outcome<int>> ExecuteAsync(
        string sql,
        object? parameters = null,
        CommandType commandType = CommandType.Text,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a query and returns a scalar value.
    /// </summary>
    Task<Outcome<T>> ExecuteScalarAsync<T>(
        string sql,
        object? parameters = null,
        CommandType commandType = CommandType.Text,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins a new database transaction.
    /// </summary>
    Task<Outcome<ITransactionScope>> BeginTransactionAsync(
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the connection factory.
    /// </summary>
    IDbConnectionFactory ConnectionFactory { get; }
}
``` 


### src > ImanSoftware.DataAccess > Abstractions > ITransactionScope 

```csharp 
﻿
using System.Data;

namespace ImanSoftware.DataAccess;

/// <summary>
/// Represents a database transaction scope.
/// </summary>
public interface ITransactionScope : IDisposable
{
    IDbTransaction Transaction { get; }
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
}
``` 


### src > ImanSoftware.DataAccess.Dapper > ImanSoftware.DataAccess.Dapper 

```xml 
﻿<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Dapper" />
    <PackageReference Include="Microsoft.Data.SqlClient" />
  </ItemGroup>


  <ItemGroup>
    <PackageReference Include="ImanSoftware.Outcome" />
    <PackageReference Include="ImanSoftware.DataAccess" />
  </ItemGroup>

  <PropertyGroup>
    <PackageId>ImanSoftware.DataAccess.Dapper</PackageId>
    <Version>1.0.0</Version>
    <Description>Dapper implementation of ImanSoftware.DataAccess for high-performance SQL operations.</Description>
    <PackageTags>dapper;dataaccess;sql;orm;micro-orm;wrapper</PackageTags>
  </PropertyGroup>


</Project>

``` 


### src > ImanSoftware.DataAccess.Dapper > DependencyInjection > DapperDataAccessRegistration 

```csharp 
﻿using Microsoft.Extensions.DependencyInjection;
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
``` 


### src > ImanSoftware.DataAccess.Dapper > Infrastructure > DapperConnectionFactory 

```csharp 
﻿// مسیر: Infrastructure/DataAccess/Dapper/DapperConnectionFactory.cs
using System.Data;
using Microsoft.Data.SqlClient;

namespace ImanSoftware.DataAccess.Dapper;

/// <summary>
/// Default implementation of IDbConnectionFactory using SqlClient.
/// </summary>
public class DapperConnectionFactory : IDbConnectionFactory
{
    public string ConnectionString { get; }

    public DapperConnectionFactory(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));

        ConnectionString = connectionString;
    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(ConnectionString);
    }
}
``` 


### src > ImanSoftware.DataAccess.Dapper > Infrastructure > DapperDataAccessor 

```csharp 
﻿// مسیر: Infrastructure/DataAccess/Dapper/DapperDataAccessor.cs
using System.Data;
using Dapper;
using ImanSoftware.Outcomes;

namespace ImanSoftware.DataAccess.Dapper;

/// <summary>
/// Default implementation of ISqlDataAccessor using Dapper.
/// </summary>
public class DapperDataAccessor : ISqlDataAccessor
{
    public IDbConnectionFactory ConnectionFactory { get; }

    public DapperDataAccessor(IDbConnectionFactory connectionFactory)
    {
        ConnectionFactory = connectionFactory;
    }

    public async Task<Outcome<T>> QuerySingleAsync<T>(
        string sql,
        object? parameters = null,
        CommandType commandType = CommandType.Text,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = ConnectionFactory.CreateConnection();
            if (connection.State != ConnectionState.Open)
                connection.Open();

            var result = await connection.QuerySingleOrDefaultAsync<T>(
                sql,
                parameters,
                transaction,
                commandType: commandType);

            return Outcome<T>.Successful(result);
        }
        catch (Exception ex)
        {
            var error = new OutcomeError(ex.Message, "DB_QUERY_ERROR", OutcomeErrorType.Failure);
            return Outcome.Failure<T>(error);
        }
    }

    public async Task<Outcome<IEnumerable<T>>> QueryAsync<T>(
        string sql,
        object? parameters = null,
        CommandType commandType = CommandType.Text,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = ConnectionFactory.CreateConnection();
            if (connection.State != ConnectionState.Open)
                connection.Open();

            var result = await connection.QueryAsync<T>(
                sql,
                parameters,
                transaction,
                commandType: commandType);

            return Outcome<IEnumerable<T>>.Successful(result);
        }
        catch (Exception ex)
        {
            var error = new OutcomeError(ex.Message, "DB_QUERY_ERROR", OutcomeErrorType.Failure);
            return Outcome.Failure<IEnumerable<T>>(error);
        }
    }

    public async Task<Outcome<int>> ExecuteAsync(
        string sql,
        object? parameters = null,
        CommandType commandType = CommandType.Text,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = ConnectionFactory.CreateConnection();
            if (connection.State != ConnectionState.Open)
                connection.Open();

            var affectedRows = await connection.ExecuteAsync(
                sql,
                parameters,
                transaction,
                commandType: commandType);

            return Outcome<int>.Successful(affectedRows);
        }
        catch (Exception ex)
        {
            var error = new OutcomeError(ex.Message, "DB_EXECUTE_ERROR", OutcomeErrorType.Failure);
            return Outcome.Failure<int>(error);
        }
    }

    public async Task<Outcome<T>> ExecuteScalarAsync<T>(
        string sql,
        object? parameters = null,
        CommandType commandType = CommandType.Text,
        IDbTransaction? transaction = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = ConnectionFactory.CreateConnection();
            if (connection.State != ConnectionState.Open)
                connection.Open();

            var result = await connection.ExecuteScalarAsync<T>(
                sql,
                parameters,
                transaction,
                commandType: commandType);

            return Outcome<T>.Successful(result);
        }
        catch (Exception ex)
        {
            var error = new OutcomeError(ex.Message, "DB_SCALAR_ERROR", OutcomeErrorType.Failure);
            return Outcome.Failure<T>(error);
        }
    }

    public async Task<Outcome<ITransactionScope>> BeginTransactionAsync(
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var connection = ConnectionFactory.CreateConnection();
            if (connection.State != ConnectionState.Open)
                connection.Open();

            var transactionScope = new DapperTransactionScope(connection, isolationLevel);
            return Outcome.Successful<ITransactionScope>(transactionScope);
        }
        catch (Exception ex)
        {
            var error = new OutcomeError(ex.Message, "DB_TRANSACTION_ERROR", OutcomeErrorType.Failure);
            return Outcome.Failure<ITransactionScope>(error);
        }
    }
}
``` 


### src > ImanSoftware.DataAccess.Dapper > Infrastructure > DapperTransactionScope 

```csharp 
﻿using System.Data;

namespace ImanSoftware.DataAccess.Dapper;

/// <summary>
/// Default implementation of ITransactionScope.
/// </summary>
public class DapperTransactionScope : ITransactionScope
{
    private readonly IDbConnection _connection;
    private IDbTransaction? _transaction;
    private bool _disposed;

    public IDbTransaction Transaction => _transaction ?? throw new InvalidOperationException("Transaction has not been started.");

    public DapperTransactionScope(IDbConnection connection, IsolationLevel isolationLevel)
    {
        _connection = connection;
        if (_connection.State != ConnectionState.Open)
            _connection.Open();

        _transaction = _connection.BeginTransaction(isolationLevel);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null)
            throw new InvalidOperationException("Transaction is null.");

        await Task.Run(() => _transaction.Commit(), cancellationToken);
        _transaction.Dispose();
        _transaction = null;
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null)
            throw new InvalidOperationException("Transaction is null.");

        await Task.Run(() => _transaction.Rollback(), cancellationToken);
        _transaction.Dispose();
        _transaction = null;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _transaction?.Dispose();
                _connection?.Dispose();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
``` 


### src > ImanSoftware.DateTimeProvider > ImanSoftware.DateTimeProvider 

```xml 
﻿<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" />
  </ItemGroup>

  <PropertyGroup>
    <PackageId>ImanSoftware.DateTimeProvider</PackageId>
    <Version>1.0.0</Version>
    <Description>Abstraction over DateTime.Now to improve testability and control over time in your application.</Description>
    <PackageTags>datetime;time;abstraction;testability;timeprovider</PackageTags>
  </PropertyGroup>
  
</Project>

``` 


### src > ImanSoftware.DateTimeProvider > Abstractions > DateTimeProviderRegistration 

```csharp 
﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ImanSoftware.DateTimeProvider;

public static class DateTimeProviderRegistration
{
    public static IServiceCollection AddDateTimeProvider(
    this IServiceCollection services, Type? customImplementation = null)
    {
        if (customImplementation is not null)
            services.AddSingleton(typeof(IDateTimeProvider), customImplementation);
        else
            services.TryAddSingleton<IDateTimeProvider, DefaultDateTimeProvider>();

        return services;
    }
}

``` 


### src > ImanSoftware.DateTimeProvider > Abstractions > IDateTimeProvider 

```csharp 
﻿

namespace ImanSoftware.DateTimeProvider
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }

        DateOnly Today { get; }

        TimeOnly CurrentTimeOfDay { get; }

    }
}

``` 


### src > ImanSoftware.DateTimeProvider > Infrastructure > DefaultDateTimeProvider 

```csharp 
﻿using ImanSoftware.DateTimeProvider;

namespace ImanSoftware.DateTimeProvider
{
    public class DefaultDateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => DateTime.Now;

        public DateOnly Today => DateOnly.FromDateTime(DateTime.Today);

        public TimeOnly CurrentTimeOfDay => TimeOnly.FromDateTime(DateTime.Now);
    }
}

``` 


### src > ImanSoftware.Domain > ImanSoftware.Domain 

```xml 
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <PropertyGroup>
    <PackageId>ImanSoftware.Domain</PackageId>
    <Version>1.0.6</Version>
    <Description>Base DDD building blocks: Entity, ValueObject, AggregateRoot, Guard clauses, and domain event abstractions.</Description>
    <PackageTags>ddd;domain;entity;valueobject;aggregate;guard;core</PackageTags>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="ImanSoftware.Outcome" />
  </ItemGroup>
</Project>
``` 


### src > ImanSoftware.Domain > Base > BaseEntity 

```csharp 
﻿// Core/Base/BaseEntity.cs
namespace ImanSoftware.Domain;

/// <summary>
/// پایه‌ترین کلاس برای موجودیت‌ها با یک شناسه‌ی جنریک.
/// </summary>
/// <typeparam name="TId">نوع شناسه (مثلاً Guid, int, long)</typeparam>
public abstract class BaseEntity<TId>
    where TId : IEquatable<TId>
{
    /// <summary>
    /// شناسه‌ی منحصربه‌فرد موجودیت.
    /// </summary>
    public TId Id { get; protected set; } = default!;

    /// <summary>
    /// سازنده‌ی پیش‌فرض (برای ORMها و Serialization).
    /// </summary>
    protected BaseEntity() { }

    /// <summary>
    /// سازنده با شناسه.
    /// </summary>
    protected BaseEntity(TId id)
    {
        Id = id;
    }

    // ===== متدهای کمکی برای مقایسه =====
    public override bool Equals(object? obj)
    {
        if (obj is not BaseEntity<TId> other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        return Id.Equals(other.Id);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static bool operator ==(BaseEntity<TId>? left, BaseEntity<TId>? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(BaseEntity<TId>? left, BaseEntity<TId>? right)
    {
        return !Equals(left, right);
    }
}
``` 


### src > ImanSoftware.Domain > Base > TaggedEntity 

```csharp 
﻿
namespace ImanSoftware.Domain;

/// <summary>
/// کلاس پایه برای موجودیت‌هایی که می‌توانند برچسب (Tag) داشته باشند.
/// </summary>
public abstract class TaggedEntity<TId> : BaseEntity<TId> where TId : IEquatable<TId>
{
    private readonly List<Tag<TId>> _tags = new();

    /// <summary>
    /// مجموعه‌ی برچسب‌های موجودیت (فقط خواندنی).
    /// </summary>
    public IReadOnlyCollection<Tag<TId>> Tags => _tags.AsReadOnly();

    protected TaggedEntity() { }
    protected TaggedEntity(TId id) : base(id) { }

    /// <summary>
    /// یک برچسب جدید به موجودیت اضافه می‌کند.
    /// </summary>
    public void AddTag(string tagName , TagType tagType)
    {
        var tag = new Tag<TId>(Id , tagName , tagType);

        // جلوگیری از اضافه شدن تگ تکراری (با ParentId و Name یکسان)
        if (_tags.Any(t => t.Name.Equals(tag.Name, StringComparison.OrdinalIgnoreCase)))
            return;

        _tags.Add(tag);
    }

    /// <summary>
    /// یک برچسب را با استفاده از اسم و ParentId حذف می‌کند.
    /// </summary>
    public bool RemoveTag(string name)
    {
        var tag = _tags.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        return tag is not null && _tags.Remove(tag);
    }

    /// <summary>
    /// همه‌ی برچسب‌ها را پاک می‌کند.
    /// </summary>
    public void ClearTags() => _tags.Clear();
}
``` 


### src > ImanSoftware.Domain > Enums > TagType 

```csharp 
﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ImanSoftware.Domain;

/// <summary>
/// Predefined tag types with their display colors.
/// </summary>
public enum TagType
{
    [Description("#007bff")] 
    Technology,

    [Description("#28a745")]
    Science,

    [Description("#dc3545")]
    Art,

    [Description("#ffc107")]
    Business,

    [Description("#17a2b8")]
    Health,

    [Description("#6f42c1")]
    Education,

    [Description("#6c757d")]
    General
}
``` 


### src > ImanSoftware.Domain > ValueObjects > Address 

```csharp 
﻿// مسیر: ValueObjects/Address.cs
using ImanSoftware.Outcomes;

namespace ImanSoftware.Domain;

public sealed record Address
{
    public string Street { get; }
    public string City { get; }
    public string State { get; }
    public string PostalCode { get; }
    public string Country { get; }

    public Address(string street, string city, string state, string postalCode, string country)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new ValidationException("Street cannot be null or empty.", ["Street is required."]);

        if (string.IsNullOrWhiteSpace(city))
            throw new ValidationException("City cannot be null or empty.", ["City is required."]);

        if (string.IsNullOrWhiteSpace(country))
            throw new ValidationException("Country cannot be null or empty.", ["Country is required."]);

        Street = street;
        City = city;
        State = state ?? string.Empty;
        PostalCode = postalCode ?? string.Empty;
        Country = country;
    }

    public override string ToString()
    {
        var parts = new[] { Street, City, State, PostalCode, Country };
        return string.Join(", ", parts.Where(p => !string.IsNullOrEmpty(p)));
    }
}
``` 


### src > ImanSoftware.Domain > ValueObjects > Coordinates 

```csharp 
﻿// مسیر: ValueObjects/Coordinates.cs
using ImanSoftware.Outcomes;

namespace ImanSoftware.Domain;

public sealed record Coordinates
{
    public double Latitude { get; }
    public double Longitude { get; }

    public Coordinates(double latitude, double longitude)
    {
        if (latitude < -90 || latitude > 90)
            throw new ValidationException($"Invalid latitude: {latitude}. Must be between -90 and 90.",
                ["Latitude out of range."]);

        if (longitude < -180 || longitude > 180)
            throw new ValidationException($"Invalid longitude: {longitude}. Must be between -180 and 180.",
                ["Longitude out of range."]);

        Latitude = Math.Round(latitude, 6);
        Longitude = Math.Round(longitude, 6);
    }

    public double DistanceTo(Coordinates other)
    {
        // فرمول هاورسین (Haversine) برای محاسبه‌ی فاصله به کیلومتر
        const double earthRadiusKm = 6371;

        var dLat = ToRadians(other.Latitude - Latitude);
        var dLon = ToRadians(other.Longitude - Longitude);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(Latitude)) * Math.Cos(ToRadians(other.Latitude)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return earthRadiusKm * c;
    }

    private static double ToRadians(double degrees) => degrees * Math.PI / 180;

    public override string ToString() => $"{Latitude:F6}, {Longitude:F6}";
}
``` 


### src > ImanSoftware.Domain > ValueObjects > DateRange 

```csharp 
﻿// مسیر: ValueObjects/DateRange.cs
using ImanSoftware.Outcomes;

namespace ImanSoftware.Domain;

public sealed record DateRange
{
    public DateTime Start { get; }
    public DateTime End { get; }

    public DateRange(DateTime start, DateTime end)
    {
        if (end < start)
            throw new ValidationException("End date cannot be before start date.", ["Invalid date range."]);

        Start = start.Date;
        End = end.Date;
    }

    public int Days => (int)(End - Start).TotalDays;

    public bool Contains(DateTime date) => date >= Start && date <= End;

    public bool Overlaps(DateRange other) =>
        Start <= other.End && other.Start <= End;

    public override string ToString() => $"{Start:yyyy-MM-dd} → {End:yyyy-MM-dd}";

    public static DateRange Day(DateTime date) => new(date, date);

    public static DateRange Month(int year, int month) =>
        new(start: new DateTime(year, month, 1), end: new DateTime(year, month, DateTime.DaysInMonth(year, month)));
}
``` 


### src > ImanSoftware.Domain > ValueObjects > EmailAddress 

```csharp 
﻿// مسیر: ValueObjects/EmailAddress.cs
using System.Text.RegularExpressions;

namespace ImanSoftware.Domain;

public sealed record EmailAddress
{
    public string Value { get; }

    public EmailAddress(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new Outcomes.ValidationException("Email address cannot be null or empty.", ["Email is required."]);

        if (!IsValid(value))
            throw new Outcomes.ValidationException($"'{value}' is not a valid email address.", ["Invalid email format."]);

        Value = value;
    }

    private static bool IsValid(string email)
    {
        // یک Regex ساده و معتبر برای ایمیل
        var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        return regex.IsMatch(email);
    }

    public override string ToString() => Value;

    public static implicit operator string(EmailAddress email) => email.Value;
    public static explicit operator EmailAddress(string value) => new(value);
}
``` 


### src > ImanSoftware.Domain > ValueObjects > Money 

```csharp 
﻿// مسیر: ValueObjects/Money.cs
using ImanSoftware.Outcomes;

namespace ImanSoftware.Domain;

public sealed record Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        if (amount < 0)
            throw new ValidationException("Amount cannot be negative.", ["Amount must be zero or positive."]);

        if (string.IsNullOrWhiteSpace(currency))
            throw new ValidationException("Currency cannot be null or empty.", ["Currency is required."]);

        if (currency.Length != 3)
            throw new ValidationException("Currency must be a 3-letter ISO code.", ["Invalid currency format."]);

        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new ValidationException($"Cannot add money with different currencies: {Currency} vs {other.Currency}.",
                ["Currency mismatch."]);

        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new ValidationException($"Cannot subtract money with different currencies: {Currency} vs {other.Currency}.",
                ["Currency mismatch."]);

        return new Money(Amount - other.Amount, Currency);
    }

    public override string ToString() => $"{Amount:F2} {Currency}";

    public static Money Zero(string currency) => new(0, currency);
}
``` 


### src > ImanSoftware.Domain > ValueObjects > PhoneNumber 

```csharp 
﻿// مسیر: ValueObjects/PhoneNumber.cs
using System.Text.RegularExpressions;
using ImanSoftware.Outcomes;

namespace ImanSoftware.Domain;

public sealed record PhoneNumber
{
    public string Value { get; }

    public PhoneNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ValidationException("Phone number cannot be null or empty.", ["Phone is required."]);

        if (!IsValid(value))
            throw new ValidationException($"'{value}' is not a valid phone number.", ["Invalid phone format."]);

        Value = value;
    }

    private static bool IsValid(string phone)
    {
        // پشتیبانی از شماره‌های بین‌المللی با + و بدون صفر ابتدایی
        var regex = new Regex(@"^\+?[1-9]\d{1,14}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        return regex.IsMatch(phone);
    }

    public override string ToString() => Value;

    public static implicit operator string(PhoneNumber phone) => phone.Value;
    public static explicit operator PhoneNumber(string value) => new(value);
}
``` 


### src > ImanSoftware.Domain > ValueObjects > Tag 

```csharp 
﻿using ImanSoftware.Outcomes;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ImanSoftware.Domain;

/// <summary>
/// Value Object برای برچسب (Tag) که به یک موجودیت والد متصل است.
/// </summary>
public sealed record Tag<TParentId> where TParentId : IEquatable<TParentId>
{
    public TParentId ParentId { get; }
    public string Name { get; }
    public TagType TagType { get; }
    public string Slug => GenerateSlug(Name);
    public string Color => GetColor(TagType);

    public Tag(TParentId parentId, string name, TagType tagType = TagType.General)
    {
        // اعتبارسنجی ParentId
        if (parentId is null || parentId.Equals(default(TParentId)))
            throw new ValidationException("ParentId cannot be null or default.", ["ParentId is required."]);

        // اعتبارسنجی Name
        if (string.IsNullOrWhiteSpace(name))
            throw new ValidationException("Tag name cannot be null or empty.", ["Tag name is required."]);

        if (name.Length > 50)
            throw new ValidationException("Tag name cannot exceed 50 characters.", ["Tag name is too long."]);

        ParentId = parentId;
        Name = name.Trim();
        TagType = tagType;
    }


    private static string GenerateSlug(string name)
    {
        var slug = name.ToLowerInvariant();
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");    // حذف کاراکترهای غیرمجاز
        slug = Regex.Replace(slug, @"\s+", "-");            // تبدیل فاصله به خط تیره
        slug = Regex.Replace(slug, @"-+", "-");             // حذف خط تیره‌های تکراری
        return slug.Trim('-');
    }

    private static string GetColor(TagType tagType)
    {
        var field = tagType.GetType().GetField(tagType.ToString());
        var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? "#6c757d"; // پیش‌فرض
    }

    public override string ToString() => $"{Name} ({Slug})";
}

``` 


### src > ImanSoftware.Extensions > ImanSoftware.Extensions 

```xml 
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <PropertyGroup>
    <PackageId>ImanSoftware.Extensions</PackageId>
    <Version>1.0.4</Version>
    <Description>Common extension methods for strings, DateTime, Enum, IEnumerable, Exception, and Object to reduce boilerplate code.</Description>
    <PackageTags>extensions;helpers;utilities;dotnet;common;core</PackageTags>
  </PropertyGroup>
</Project>
``` 


### src > ImanSoftware.Extensions > Extensions > DateTimeExtensions 

```csharp 
﻿// Core/Extensions/DateTimeExtensions.cs
using System.Globalization;

namespace ImanSoftware.Extensions;

public static class DateTimeExtensions
{
    /// <summary>
    /// Gets the start of the day (00:00:00).
    /// </summary>
    public static DateTime StartOfDay(this DateTime dateTime)
        => dateTime.Date;

    /// <summary>
    /// Gets the end of the day (23:59:59.999).
    /// </summary>
    public static DateTime EndOfDay(this DateTime dateTime)
        => dateTime.Date.AddDays(1).AddTicks(-1);

    /// <summary>
    /// Gets the start of the week (default: Monday).
    /// </summary>
    public static DateTime StartOfWeek(this DateTime dateTime, DayOfWeek startOfWeek = DayOfWeek.Monday)
    {
        int diff = (7 + (dateTime.DayOfWeek - startOfWeek)) % 7;
        return dateTime.AddDays(-1 * diff).Date;
    }

    /// <summary>
    /// Gets the end of the week (Sunday).
    /// </summary>
    public static DateTime EndOfWeek(this DateTime dateTime, DayOfWeek startOfWeek = DayOfWeek.Monday)
        => dateTime.StartOfWeek(startOfWeek).AddDays(6).EndOfDay();

    /// <summary>
    /// Gets the number of days in the month.
    /// </summary>
    public static int DaysInMonth(this DateTime dateTime)
        => DateTime.DaysInMonth(dateTime.Year, dateTime.Month);

    /// <summary>
    /// Checks if the date is today.
    /// </summary>
    public static bool IsToday(this DateTime dateTime)
        => dateTime.Date == DateTime.Today;

    /// <summary>
    /// Checks if the date is in the future.
    /// </summary>
    public static bool IsFuture(this DateTime dateTime)
        => dateTime > DateTime.Now;

    /// <summary>
    /// Checks if the date is in the past.
    /// </summary>
    public static bool IsPast(this DateTime dateTime)
        => dateTime < DateTime.Now;

    /// <summary>
    /// Converts a DateTime to a UNIX timestamp (seconds since 1970-01-01).
    /// </summary>
    public static long ToUnixTimestamp(this DateTime dateTime)
        => new DateTimeOffset(dateTime.ToUniversalTime()).ToUnixTimeSeconds();

    /// <summary>
    /// Converts a UNIX timestamp to a DateTime.
    /// </summary>
    public static DateTime FromUnixTimestamp(long timestamp)
        => DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime;

    /// <summary>
    /// Returns the age in years from a birthdate.
    /// </summary>
    public static int Age(this DateTime birthDate)
    {
        var today = DateTime.Today;
        var age = today.Year - birthDate.Year;
        if (birthDate.Date > today.AddYears(-age)) age--;
        return age;
    }

    private static readonly PersianCalendar PersianCalendar = new();

    /// <summary>
    /// Converts a DateTime? to a Persian (Solar Hijri) date string.
    /// Returns defaultValue if the input is null.
    /// </summary>
    /// <param name="dateTime">The nullable DateTime to convert.</param>
    /// <param name="separator">Separator between year, month, and day (default: "/").</param>
    /// <param name="defaultValue">Value to return if DateTime is null (default: string.Empty).</param>
    public static string ToPersianDateString(
        this DateTime? dateTime,
        string separator = "/",
        string defaultValue = "")
    {
        if (!dateTime.HasValue)
            return defaultValue;

        int year = PersianCalendar.GetYear(dateTime.Value);
        int month = PersianCalendar.GetMonth(dateTime.Value);
        int day = PersianCalendar.GetDayOfMonth(dateTime.Value);
        return $"{year}{separator}{month:D2}{separator}{day:D2}";
    }

    /// <summary>
    /// Converts a Persian date string (yyyy/MM/dd) to DateTime.
    /// </summary>
    public static DateTime? ToGregorianDateTime(this string persianDate, string separator = "/")
    {
        var parts = persianDate.Split(separator);
        if (parts.Length != 3)
            return null;

        if (!int.TryParse(parts[0], out int year) ||
            !int.TryParse(parts[1], out int month) ||
            !int.TryParse(parts[2], out int day))
            return null;

        try
        {
            return PersianCalendar.ToDateTime(year, month, day, 0, 0, 0, 0);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Gets the Persian month name.
    /// </summary>
    public static string GetPersianMonthName(this DateTime? dateTime)
    {
        if (!dateTime.HasValue)
            return string.Empty; // یا "نامشخص"

        int month = PersianCalendar.GetMonth(dateTime.Value);
        return month switch
        {
            1 => "فروردین",
            2 => "اردیبهشت",
            3 => "خرداد",
            4 => "تیر",
            5 => "مرداد",
            6 => "شهریور",
            7 => "مهر",
            8 => "آبان",
            9 => "آذر",
            10 => "دی",
            11 => "بهمن",
            12 => "اسفند",
            _ => string.Empty
        };
    }
}

``` 


### src > ImanSoftware.Extensions > Extensions > EnumerableExtensions 

```csharp 
﻿// Core/Extensions/EnumerableExtensions.cs
namespace ImanSoftware.Extensions;

public static class EnumerableExtensions
{
    /// <summary>
    /// Checks if an enumerable is null or empty.
    /// </summary>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source)
        => source is null || !source.Any();

    /// <summary>
    /// Converts an enumerable to a HashSet.
    /// </summary>
    public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        => new(source);

    /// <summary>
    /// Performs an action for each item in an enumerable.
    /// </summary>
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var item in source)
            action(item);
    }

    /// <summary>
    /// Returns distinct elements based on a specified key selector.
    /// </summary>
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector)
    {
        var seenKeys = new HashSet<TKey>();
        foreach (var element in source)
        {
            if (seenKeys.Add(keySelector(element)))
                yield return element;
        }
    }

    /// <summary>
    /// Splits an enumerable into chunks of a specified size.
    /// </summary>
    public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunkSize)
    {
        if (chunkSize <= 0)
            throw new ArgumentException("Chunk size must be positive.", nameof(chunkSize));

        var chunk = new List<T>(chunkSize);
        foreach (var item in source)
        {
            chunk.Add(item);
            if (chunk.Count == chunkSize)
            {
                yield return chunk;
                chunk = new List<T>(chunkSize);
            }
        }
        if (chunk.Any())
            yield return chunk;
    }
}
``` 


### src > ImanSoftware.Extensions > Extensions > EnumExtensions 

```csharp 
﻿// Core/Extensions/EnumExtensions.cs
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace ImanSoftware.Extensions;

public static class EnumExtensions
{
    /// <summary>
    /// Gets the Description attribute value of an enum value.
    /// </summary>
    public static string GetDescription(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? value.ToString();
    }

    /// <summary>
    /// Gets the Display attribute value of an enum value.
    /// </summary>
    public static string GetDisplayName(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = field?.GetCustomAttribute<DisplayAttribute>();
        return attribute?.Name ?? value.ToString();
    }

    /// <summary>
    /// Checks if the enum value is defined in the enum.
    /// </summary>
    public static bool IsDefined<TEnum>(this TEnum value) where TEnum : struct, Enum
        => Enum.IsDefined(typeof(TEnum), value);

    /// <summary>
    /// Gets all values of an enum type.
    /// </summary>
    public static IEnumerable<TEnum> GetValues<TEnum>() where TEnum : struct, Enum
        => Enum.GetValues(typeof(TEnum)).Cast<TEnum>();

    /// <summary>
    /// Converts an enum to its underlying integral value.
    /// </summary>
    public static int ToInt(this Enum value)
        => Convert.ToInt32(value);

    /// <summary>
    /// Parses a string to an enum value (case-insensitive).
    /// </summary>
    public static TEnum Parse<TEnum>(string value) where TEnum : struct, Enum
        => (TEnum)Enum.Parse(typeof(TEnum), value, true);

    /// <summary>
    /// Converts an enum value to a human-readable string by replacing underscores with spaces.
    /// </summary>
    public static string EnumToString(this Enum? value)
        => value?.ToString().Replace("_", " ") ?? string.Empty;
}
``` 


### src > ImanSoftware.Extensions > Extensions > ExceptionExtensions 

```csharp 
﻿// Core/Extensions/ExceptionExtensions.cs
using System.Text;

namespace ImanSoftware.Extensions;

public static class ExceptionExtensions
{
    /// <summary>
    /// Gets the full error message including inner exceptions.
    /// </summary>
    public static string GetFullMessage(this Exception ex)
    {
        if (ex is null)
            return string.Empty;

        var sb = new StringBuilder();
        var current = ex;
        while (current is not null)
        {
            sb.AppendLine(current.Message);
            current = current.InnerException;
        }
        return sb.ToString();
    }

    /// <summary>
    /// Gets the full stack trace including inner exceptions.
    /// </summary>
    public static string GetFullStackTrace(this Exception ex)
    {
        if (ex is null)
            return string.Empty;

        var sb = new StringBuilder();
        var current = ex;
        while (current is not null)
        {
            sb.AppendLine(current.StackTrace);
            current = current.InnerException;
        }
        return sb.ToString();
    }

    /// <summary>
    /// Checks if an exception is of a specific type or its derived types.
    /// </summary>
    public static bool Is<TException>(this Exception ex) where TException : Exception
        => ex is TException;

    /// <summary>
    /// Recursively finds the most inner exception.
    /// </summary>
    public static Exception GetInnermostException(this Exception ex)
    {
        var current = ex;
        while (current.InnerException is not null)
            current = current.InnerException;
        return current;
    }
}
``` 


### src > ImanSoftware.Extensions > Extensions > ObjectExtensions 

```csharp 
﻿// Core/Extensions/ObjectExtensions.cs
using System.Text.Json;

namespace ImanSoftware.Extensions;

public static class ObjectExtensions
{
    /// <summary>
    /// Converts an object to a JSON string.
    /// </summary>
    public static string ToJson(this object obj, bool indented = false)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = indented,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        return JsonSerializer.Serialize(obj, options);
    }

    /// <summary>
    /// Checks if an object is null.
    /// </summary>
    public static bool IsNull(this object? obj)
        => obj is null;

    /// <summary>
    /// Throws an ArgumentNullException if the object is null.
    /// </summary>
    public static T ThrowIfNull<T>(this T? obj, string paramName) where T : class
    {
        if (obj is null)
            throw new ArgumentNullException(paramName);
        return obj;
    }

    /// <summary>
    /// Creates a shallow copy of an object using serialization.
    /// </summary>
    public static T? DeepCopy<T>(this T obj) where T : class
    {
        var json = JsonSerializer.Serialize(obj);
        return JsonSerializer.Deserialize<T>(json);
    }
}
``` 


### src > ImanSoftware.Extensions > Extensions > StringExtensions 

```csharp 
﻿
// Core/Extensions/StringExtensions.cs
using System.Text.RegularExpressions;

namespace ImanSoftware.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Checks if a string is null, empty, or whitespace.
    /// </summary>
    public static bool IsNullOrWhiteSpace(this string? value)
        => string.IsNullOrWhiteSpace(value);

    /// <summary>
    /// Checks if a string is null or empty.
    /// </summary>
    public static bool IsNullOrEmpty(this string? value)
        => string.IsNullOrEmpty(value);

    /// <summary>
    /// Truncates a string to a maximum length with an optional suffix.
    /// </summary>
    public static string Truncate(this string value, int maxLength, string suffix = "...")
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : value[..(maxLength - suffix.Length)] + suffix;
    }

    /// <summary>
    /// Converts a string to camelCase.
    /// </summary>
    public static string ToCamelCase(this string value)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return char.ToLowerInvariant(value[0]) + value[1..];
    }

    /// <summary>
    /// Converts a string to snake_case.
    /// </summary>
    public static string ToSnakeCase(this string value)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return Regex.Replace(value, "([a-z0-9])([A-Z])", "$1_$2").ToLowerInvariant();
    }

    /// <summary>
    /// Converts a string to kebab-case.
    /// </summary>
    public static string ToKebabCase(this string value)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return Regex.Replace(value, "([a-z0-9])([A-Z])", "$1-$2").ToLowerInvariant();
    }

    /// <summary>
    /// Checks if a string is a valid email address.
    /// </summary>
    public static bool IsEmail(this string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;
        try
        {
            var addr = new System.Net.Mail.MailAddress(value);
            return addr.Address == value;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Extracts numbers from a string.
    /// </summary>
    public static string ExtractNumbers(this string value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        return new string(value.Where(char.IsDigit).ToArray());
    }

    /// <summary>
    /// Replaces multiple spaces with a single space.
    /// </summary>
    public static string NormalizeWhitespace(this string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return value;
        return Regex.Replace(value, @"\s+", " ").Trim();
    }
}
``` 


### src > ImanSoftware.FileStorage > ImanSoftware.FileStorage 

```xml 
﻿<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" />
    <PackageReference Include="Microsoft.Extensions.Options" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="ImanSoftware.Logging"/>
    <PackageReference Include="ImanSoftware.Outcome" />
  </ItemGroup>
  
  <PropertyGroup>
    <PackageId>ImanSoftware.FileStorage</PackageId>
    <Version>1.0.0</Version>
    <Description>File storage abstraction for upload, download, delete, move, copy, and metadata operations (local file system included).</Description>
    <PackageTags>filestorage;upload;download;storage;io</PackageTags>
  </PropertyGroup>
  
</Project>

``` 


### src > ImanSoftware.FileStorage > Abstractions > FileStorageRegistration 

```csharp 
﻿// مسیر: Abstractions/FileStorage/FileStorageRegistration.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ImanSoftware.FileStorage;

public static class FileStorageRegistration
{
    public static IServiceCollection AddFileStorage(
        this IServiceCollection services,
        Action<LocalFileStorageOptions> configureOptions,
        Action<IServiceCollection>? customRegistration = null)
    {
        ArgumentNullException.ThrowIfNull(configureOptions);

        services.Configure(configureOptions);

        if (customRegistration is not null)
        {
            customRegistration(services);
        }
        else
        {
            // main
            services.TryAddScoped<IFileStorage, LocalFileStorage>();

            // small interfaces
            services.TryAddScoped<IFileUploader>(sp => sp.GetRequiredService<IFileStorage>());
            services.TryAddScoped<IFileDownloader>(sp => sp.GetRequiredService<IFileStorage>());
            services.TryAddScoped<IFileDeleter>(sp => sp.GetRequiredService<IFileStorage>());
            services.TryAddScoped<IFileInfoProvider>(sp => sp.GetRequiredService<IFileStorage>());
            services.TryAddScoped<IFileMover>(sp => sp.GetRequiredService<IFileStorage>());
        }

        return services;
    }
}
``` 


### src > ImanSoftware.FileStorage > Abstractions > IFileDeleter 

```csharp 
﻿// مسیر: Abstractions/FileStorage/IFileDeleter.cs
using ImanSoftware.Outcomes;

namespace ImanSoftware.FileStorage;

/// <summary>
/// Contract for deleting files from a storage system.
/// </summary>
public interface IFileDeleter
{
    /// <summary>
    /// Deletes a file from the storage.
    /// </summary>
    Task<Outcome> DeleteAsync(string filePath);
}
``` 


### src > ImanSoftware.FileStorage > Abstractions > IFileDownloader 

```csharp 
﻿// مسیر: Abstractions/FileStorage/IFileDownloader.cs
using ImanSoftware.Outcomes;

namespace ImanSoftware.FileStorage;

/// <summary>
/// Contract for downloading files from a storage system.
/// </summary>
public interface IFileDownloader
{
    /// <summary>
    /// Downloads a file as a stream.
    /// </summary>
    Task<Outcome<Stream>> DownloadAsync(string filePath);

    /// <summary>
    /// Downloads a file as a byte array.
    /// </summary>
    Task<Outcome<byte[]>> DownloadBytesAsync(string filePath);
}
``` 


### src > ImanSoftware.FileStorage > Abstractions > IFileInfoProvider 

```csharp 
﻿// مسیر: Abstractions/FileStorage/IFileInfoProvider.cs
using ImanSoftware.FileStorage;
using ImanSoftware.Outcomes;

namespace ImanSoftware.FileStorage;

/// <summary>
/// Contract for retrieving file information and metadata.
/// </summary>
public interface IFileInfoProvider
{
    /// <summary>
    /// Checks if a file exists in the storage.
    /// </summary>
    Task<Outcome<bool>> ExistsAsync(string filePath);

    /// <summary>
    /// Retrieves metadata information of a file.
    /// </summary>
    Task<Outcome<FileMetadata>> GetMetadataAsync(string filePath);

    /// <summary>
    /// Gets a public URL to access the file (if supported).
    /// </summary>
    Task<Outcome<string>> GetUrlAsync(string filePath);
}
``` 


### src > ImanSoftware.FileStorage > Abstractions > IFileMover 

```csharp 
﻿// مسیر: Abstractions/FileStorage/IFileMover.cs
using ImanSoftware.Outcomes;

namespace ImanSoftware.FileStorage;

/// <summary>
/// Contract for moving and copying files.
/// </summary>
public interface IFileMover
{
    /// <summary>
    /// Moves a file from source to destination.
    /// </summary>
    Task<Outcome> MoveAsync(string sourcePath, string destinationPath);

    /// <summary>
    /// Copies a file from source to destination.
    /// </summary>
    Task<Outcome> CopyAsync(string sourcePath, string destinationPath);
}
``` 


### src > ImanSoftware.FileStorage > Abstractions > IFileStorage 

```csharp 
﻿// مسیر: Abstractions/FileStorage/IFileStorage.cs
namespace ImanSoftware.FileStorage;

/// <summary>
/// Combined contract for full file storage operations.
/// Inherits all individual file operation interfaces.
/// </summary>
public interface IFileStorage :
    IFileUploader,
    IFileDownloader,
    IFileDeleter,
    IFileInfoProvider,
    IFileMover
{
    // No additional methods needed - just composition
}
``` 


### src > ImanSoftware.FileStorage > Abstractions > IFileUploader 

```csharp 
﻿// مسیر: Abstractions/FileStorage/IFileUploader.cs
using ImanSoftware.Outcomes;

namespace ImanSoftware.FileStorage;

/// <summary>
/// Contract for uploading files to a storage system.
/// </summary>
public interface IFileUploader
{
    /// <summary>
    /// Uploads a file from a stream.
    /// </summary>
    /// <param name="fileStream">The file content as a stream.</param>
    /// <param name="fileName">The name of the file (including extension).</param>
    /// <param name="container">Optional container/folder name.</param>
    /// <returns>The path or identifier of the stored file.</returns>
    Task<Outcome<string>> UploadAsync(Stream fileStream, string fileName, string? container = null);

    /// <summary>
    /// Uploads a file from a byte array.
    /// </summary>
    Task<Outcome<string>> UploadAsync(byte[] fileBytes, string fileName, string? container = null);
}
``` 


### src > ImanSoftware.FileStorage > Core > FileMetadata 

```csharp 
﻿// مسیر: Core/FileStorage/FileMetadata.cs
namespace ImanSoftware.FileStorage;

/// <summary>
/// Represents metadata information of a stored file.
/// </summary>
public class FileMetadata
{
    public string FileName { get; set; } = string.Empty;
    public long Size { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public Dictionary<string, object>? CustomProperties { get; set; }
}
``` 


### src > ImanSoftware.FileStorage > Infrastructure > LocalFileStorage 

```csharp 
﻿// مسیر: Infrastructure/FileStorage/LocalFileStorage.cs
using ImanSoftware.FileStorage;
using ImanSoftware.Logging;
using ImanSoftware.Outcomes;
using Microsoft.Extensions.Options;
using System.Security;

namespace ImanSoftware.FileStorage;

/// <summary>
/// Default implementation of IFileStorage using the local file system.
/// </summary>
public class LocalFileStorage : IFileStorage
{
    private readonly LocalFileStorageOptions _options;
    private readonly ILoggerAdapter<LocalFileStorage> _logger;

    public LocalFileStorage(IOptions<LocalFileStorageOptions> options, ILoggerAdapter<LocalFileStorage> logger)
    {
        _options = options.Value;
        _logger = logger;

        // ایجاد پوشه‌ی ریشه در صورت عدم وجود
        if (!Directory.Exists(_options.RootPath))
        {
            Directory.CreateDirectory(_options.RootPath);
        }
    }

    #region IFileUploader Implementation

    public async Task<Outcome<string>> UploadAsync(Stream fileStream, string fileName, string? container = null)
    {
        try
        {
            // اعتبارسنجی
            var validationResult = ValidateFile(fileName, fileStream.Length);
            if (!validationResult.IsSuccess)
                return Outcome.Failure<string>(validationResult.Error);

            var filePath = GenerateFilePath(fileName, container);
            var fullPath = Path.Combine(_options.RootPath, filePath);

            // ایجاد پوشه‌ی مقصد در صورت عدم وجود
            var directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // ذخیره‌سازی فایل
            using var fileStreamOutput = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
            await fileStream.CopyToAsync(fileStreamOutput);

            _logger.LogInformation("File uploaded successfully: {FilePath}", filePath);
            return Outcome<string>.Successful(filePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file: {FileName}", fileName);
            return Outcome.Failure<string>(new OutcomeError(ex.Message, "FILE_UPLOAD_ERROR", OutcomeErrorType.Failure));
        }
    }

    public async Task<Outcome<string>> UploadAsync(byte[] fileBytes, string fileName, string? container = null)
    {
        using var stream = new MemoryStream(fileBytes);
        return await UploadAsync(stream, fileName, container);
    }

    #endregion

    #region IFileDownloader Implementation

    public async Task<Outcome<Stream>> DownloadAsync(string filePath)
    {
        try
        {
            var fullPath = GetFullPath(filePath);
            if (!File.Exists(fullPath))
                return Outcome.Failure<Stream>(new OutcomeError("File not found.", "FILE_NOT_FOUND", OutcomeErrorType.NotFound));

            var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            return Outcome.Successful<Stream>(stream);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading file: {FilePath}", filePath);
            return Outcome.Failure<Stream>(new OutcomeError(ex.Message, "FILE_DOWNLOAD_ERROR", OutcomeErrorType.Failure));
        }
    }

    public async Task<Outcome<byte[]>> DownloadBytesAsync(string filePath)
    {
        var result = await DownloadAsync(filePath);
        if (!result.IsSuccess)
            return Outcome.Failure<byte[]>(result.Error);

        try
        {
            using var memoryStream = new MemoryStream();
            await result.Data.CopyToAsync(memoryStream);
            return Outcome<byte[]>.Successful(memoryStream.ToArray());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading file bytes: {FilePath}", filePath);
            return Outcome.Failure<byte[]>(new OutcomeError(ex.Message, "FILE_READ_ERROR", OutcomeErrorType.Failure));
        }
        finally
        {
            result.Data?.Dispose();
        }
    }

    #endregion

    #region IFileDeleter Implementation

    public Task<Outcome> DeleteAsync(string filePath)
    {
        try
        {
            var fullPath = GetFullPath(filePath);
            if (!File.Exists(fullPath))
                return Task.FromResult(Outcome.Failure(new OutcomeError("File not found.", "FILE_NOT_FOUND", OutcomeErrorType.NotFound)));

            File.Delete(fullPath);
            _logger.LogInformation("File deleted successfully: {FilePath}", filePath);
            return Task.FromResult(Outcome.Successful());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file: {FilePath}", filePath);
            return Task.FromResult(Outcome.Failure(new OutcomeError(ex.Message, "FILE_DELETE_ERROR", OutcomeErrorType.Failure)));
        }
    }

    #endregion

    #region IFileInfoProvider Implementation

    public Task<Outcome<bool>> ExistsAsync(string filePath)
    {
        try
        {
            var fullPath = GetFullPath(filePath);
            return Task.FromResult(Outcome<bool>.Successful(File.Exists(fullPath)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking file existence: {FilePath}", filePath);
            return Task.FromResult(Outcome.Failure<bool>(new OutcomeError(ex.Message, "FILE_EXISTS_ERROR", OutcomeErrorType.Failure)));
        }
    }

    public Task<Outcome<FileMetadata>> GetMetadataAsync(string filePath)
    {
        try
        {
            var fullPath = GetFullPath(filePath);
            if (!File.Exists(fullPath))
                return Task.FromResult(Outcome.Failure<FileMetadata>(new OutcomeError("File not found.", "FILE_NOT_FOUND", OutcomeErrorType.NotFound)));

            var fileInfo = new FileInfo(fullPath);
            var metadata = new FileMetadata
            {
                FileName = Path.GetFileName(filePath),
                Size = fileInfo.Length,
                ContentType = GetContentType(Path.GetExtension(filePath)),
                CreatedAt = fileInfo.CreationTime,
                LastModifiedAt = fileInfo.LastWriteTime
            };

            return Task.FromResult(Outcome.Successful<FileMetadata>(metadata));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting file metadata: {FilePath}", filePath);
            return Task.FromResult(Outcome.Failure<FileMetadata>(new OutcomeError(ex.Message, "FILE_METADATA_ERROR", OutcomeErrorType.Failure)));
        }
    }

    public Task<Outcome<string>> GetUrlAsync(string filePath)
    {
        // در Local File Storage، معمولاً URL نسبی یا کامل برمی‌گردد
        // این بستگی به نحوه‌ی تنظیم Static Files در پروژه دارد
        var url = $"/uploads/{filePath.Replace('\\', '/')}";
        return Task.FromResult(Outcome<string>.Successful(url));
    }

    #endregion

    #region IFileMover Implementation

    public Task<Outcome> MoveAsync(string sourcePath, string destinationPath)
    {
        try
        {
            var fullSource = GetFullPath(sourcePath);
            var fullDest = GetFullPath(destinationPath);

            if (!File.Exists(fullSource))
                return Task.FromResult(Outcome.Failure(new OutcomeError("Source file not found.", "FILE_NOT_FOUND", OutcomeErrorType.NotFound)));

            // ایجاد پوشه‌ی مقصد
            var destDirectory = Path.GetDirectoryName(fullDest);
            if (!string.IsNullOrEmpty(destDirectory) && !Directory.Exists(destDirectory))
            {
                Directory.CreateDirectory(destDirectory);
            }

            File.Move(fullSource, fullDest);
            _logger.LogInformation("File moved successfully: {Source} → {Destination}", sourcePath, destinationPath);
            return Task.FromResult(Outcome.Successful());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moving file: {Source} → {Destination}", sourcePath, destinationPath);
            return Task.FromResult(Outcome.Failure(new OutcomeError(ex.Message, "FILE_MOVE_ERROR", OutcomeErrorType.Failure)));
        }
    }

    public Task<Outcome> CopyAsync(string sourcePath, string destinationPath)
    {
        try
        {
            var fullSource = GetFullPath(sourcePath);
            var fullDest = GetFullPath(destinationPath);

            if (!File.Exists(fullSource))
                return Task.FromResult(Outcome.Failure(new OutcomeError("Source file not found.", "FILE_NOT_FOUND", OutcomeErrorType.NotFound)));

            var destDirectory = Path.GetDirectoryName(fullDest);
            if (!string.IsNullOrEmpty(destDirectory) && !Directory.Exists(destDirectory))
            {
                Directory.CreateDirectory(destDirectory);
            }

            File.Copy(fullSource, fullDest, overwrite: true);
            _logger.LogInformation("File copied successfully: {Source} → {Destination}", sourcePath, destinationPath);
            return Task.FromResult(Outcome.Successful());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error copying file: {Source} → {Destination}", sourcePath, destinationPath);
            return Task.FromResult(Outcome.Failure(new OutcomeError(ex.Message, "FILE_COPY_ERROR", OutcomeErrorType.Failure)));
        }
    }

    #endregion

    #region Private Methods

    private string GetFullPath(string filePath)
    {
        // جلوگیری از Path Traversal attacks
        var normalizedPath = Path.GetFullPath(Path.Combine(_options.RootPath, filePath));
        if (!normalizedPath.StartsWith(_options.RootPath, StringComparison.OrdinalIgnoreCase))
            throw new SecurityException("Invalid path traversal attempt.");

        return normalizedPath;
    }

    private string GenerateFilePath(string fileName, string? container)
    {
        // ایجاد نام فایل یکتا با GUID
        var extension = Path.GetExtension(fileName);
        var uniqueName = $"{Guid.NewGuid():N}{extension}";

        // ساخت مسیر
        var path = string.Empty;
        if (_options.UseDateSubdirectories)
        {
            var now = DateTime.Now;
            path = Path.Combine(now.ToString("yyyy"), now.ToString("MM"), now.ToString("dd"));
        }

        if (!string.IsNullOrEmpty(container))
        {
            path = Path.Combine(path, container);
        }

        return Path.Combine(path, uniqueName);
    }

    private Outcome ValidateFile(string fileName, long fileSize)
    {
        // بررسی حجم فایل
        if (fileSize > _options.MaxFileSize)
        {
            var sizeInMB = _options.MaxFileSize / (1024 * 1024);
            return Outcome.Failure(new OutcomeError(
                $"File size exceeds maximum allowed ({sizeInMB} MB).",
                "FILE_TOO_LARGE",
                OutcomeErrorType.Validation));
        }

        // بررسی پسوند فایل
        if (_options.AllowedExtensions.Length > 0)
        {
            var extension = Path.GetExtension(fileName);
            if (!_options.AllowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
            {
                return Outcome.Failure(new OutcomeError(
                    $"File extension '{extension}' is not allowed.",
                    "FILE_EXTENSION_NOT_ALLOWED",
                    OutcomeErrorType.Validation));
            }
        }

        return Outcome.Successful();
    }

    private string GetContentType(string extension)
    {
        return extension.ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".txt" => "text/plain",
            ".json" => "application/json",
            ".xml" => "application/xml",
            _ => "application/octet-stream"
        };
    }

    #endregion
}
``` 


### src > ImanSoftware.FileStorage > Infrastructure > LocalFileStorageOptions 

```csharp 
﻿// مسیر: Infrastructure/FileStorage/LocalFileStorageOptions.cs
namespace ImanSoftware.FileStorage;

/// <summary>
/// Configuration options for LocalFileStorage.
/// </summary>
public class LocalFileStorageOptions
{
    /// <summary>
    /// Root directory for storing files (e.g., "wwwroot/uploads").
    /// </summary>
    public string RootPath { get; set; } = "wwwroot/uploads";

    /// <summary>
    /// Maximum allowed file size in bytes (default: 100 MB).
    /// </summary>
    public long MaxFileSize { get; set; } = 100 * 1024 * 1024;

    /// <summary>
    /// Allowed file extensions (e.g., ".jpg", ".png", ".pdf"). Empty means all allowed.
    /// </summary>
    public string[] AllowedExtensions { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Whether to create subdirectories based on date (e.g., 2024/01/15/).
    /// </summary>
    public bool UseDateSubdirectories { get; set; } = true;
}
``` 


### src > ImanSoftware.Framework > ImanSoftware.Framework 

```xml 
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="ImanSoftware.Outcome" />
    <PackageReference Include="ImanSoftware.Domain" />
    <PackageReference Include="ImanSoftware.Extensions" />
    <PackageReference Include="ImanSoftware.Caching" />
    <PackageReference Include="ImanSoftware.CurrentUserAccessor" />
    <PackageReference Include="ImanSoftware.DataAccess.Dapper" />
    <PackageReference Include="ImanSoftware.DateTimeProvider" />
    <PackageReference Include="ImanSoftware.FileStorage" />
    <PackageReference Include="ImanSoftware.GuidGenerator" />
    <PackageReference Include="ImanSoftware.Jwt" />
    <PackageReference Include="ImanSoftware.Logging" />
    <PackageReference Include="ImanSoftware.Serialization" />
  </ItemGroup>
  <PropertyGroup>
    <PackageId>ImanSoftware.Framework</PackageId>
    <Version>1.0.7</Version>
    <Description>Umbrella package that bundles all ImanSoftware modules for a unified development experience.</Description>
    <PackageTags>framework;all-in-one;umbrella;dotnet;imansoftware</PackageTags>
  </PropertyGroup>
</Project>
``` 


### src > ImanSoftware.Framework > FrameworkDependencyInjection > FrameworkDependencyInjection 

```csharp 
﻿using ImanSoftware.Caching;
using ImanSoftware.CurrentUserAccessor;
using ImanSoftware.DataAccess;
using ImanSoftware.DataAccess.Dapper;
using ImanSoftware.DateTimeProvider;
using ImanSoftware.FileStorage;
using ImanSoftware.GuidGenerator;
using ImanSoftware.Jwt;
using ImanSoftware.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace ImanSoftware.Framework.FrameworkDependencyInjection;


/// <summary>
/// Central entry point for registering all ImanFramework services.
/// Uses internal registration classes from each abstraction module.
/// </summary>
public static class FrameworkDependencyInjection
{
    /// <summary>
    /// Registers all enabled ImanFramework services into the DI container.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="options">Configuration options for the framework.</param>
    /// <returns>The modified IServiceCollection for chaining.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when a service is enabled but no registration (default or custom) is available.
    /// </exception>
    public static IServiceCollection AddImanFramework(
        this IServiceCollection services,
        ImanFrameworkDependencyInjectionOptions options)
    {
        if (options is null)
            options = new ImanFrameworkDependencyInjectionOptions();

        // 1. DateTime Provider
        if (options.AddDateTimeProvider)
        {
            DateTimeProviderRegistration.AddDateTimeProvider(
                services,
                options.DateTimeProviderCustomImplementation?.CustomImplementation);
        }

        // 2. Guid Generator
        if (options.AddGuidGenerator)
        {
            GuidGeneratorRegistration.AddGuidGenerator(
                services,
                options.GuidGeneratorImplementationType,
                options.GuidGeneratorCustomImplementation?.CustomImplementation);
        }

        // 3. Current User Accessor - Default Is Open Generic (Must have custom registration)
        if (options.AddCurrentUserAccessor)
        {
            CurrentUserAccessorRegistration.AddCurrentUserAccessor(
                services,
                options.CurrentUserAccessorCustomRegistration);
        }

        // 4. JsonSerializer
        if (options.AddJsonSerializer)
        {
            ImanJsonSerializerRegistration.AddImanSerialization(
                services,
                options.SerializationImplementationType,
                options.JsonSerializerCustomImplementation?.CustomImplementation);
        }

        // 5. CacheService
        if (options.AddCacheService)
        {
            CacheServiceRegistration.AddCacheService(
                services,
                options.CacheServiceCustomImplementation?.CustomImplementation);
        }

        // 6. JwtService
        if (options.AddJwtService)
        {
            if (options.JwtCustomImplementation is null && options.JwtConfigureOptions is null)
            {
                throw new ArgumentException(message: "ImanSoftware.Abstractions.Jwt.IJwtService Needs Options For Registering Default Implementation.", paramName: nameof(options.JwtConfigureOptions));
            }
            JwtTokenServiceRegistration.AddJwtTokenService(
                services,
                options.JwtConfigureOptions,
                options.JwtCustomImplementation?.CustomImplementation);
        }

        // 7. DataAccess
        if (options.AddDataAccess)
        {
            // When using the default DataAccess implementation, ConnectionString must be provided.
            ArgumentException.ThrowIfNullOrWhiteSpace(options.DataAccessConnectionString, nameof(options.DataAccessConnectionString));


            DataAccessRegistration.AddDataAccess(
                services,
                options.DataAccessCustomRegistration);
        }

        // 7.1. DataAccess.Dapper
        if (options.AddDapperDataAccess)
        {

            // When using the default DataAccess implementation, ConnectionString must be provided.
            ArgumentException.ThrowIfNullOrWhiteSpace(options.DapperConnectionString, nameof(options.DapperConnectionString));


            DapperDataAccessRegistration.AddDapperDataAccess(
                services,
                options.DapperConnectionString);
        }

        // 8. FileStorage
        if (options.AddFileStorage)
        {
            if (options.FileStorageCustomRegistration is null && options.FileStorageConfigureOptions is null)
            {
                throw new ArgumentException("When using the default FileStorage implementation, FileStorageConfigureOptions must be provided. ");
            }

            FileStorageRegistration.AddFileStorage(
                services,
                options.FileStorageConfigureOptions!,
                options.FileStorageCustomRegistration);
        }


        return services;
    }
}
``` 


### src > ImanSoftware.Framework > FrameworkDependencyInjection > ImanFrameworkDependencyInjectionCustomImplementation 

```csharp 
﻿

namespace ImanSoftware.Framework.FrameworkDependencyInjection;

public class ImanFrameworkDependencyInjectionCustomImplementation<TInterface>
{
    private Type? customImplementation;

    public Type? CustomImplementation {
        get => customImplementation;
        set
        {
            if(value is not null && typeof(TInterface).IsAssignableFrom(value))
                customImplementation = value;
        }
    }


    /// <summary>
    /// Implicitly converts a Type to an instance of this class.
    /// Allows writing: options.JwtCustomImplementation = typeof(MyCustomJwtService);
    /// </summary>
    public static implicit operator ImanFrameworkDependencyInjectionCustomImplementation<TInterface>(Type type)
    {
        return new()
        {
            CustomImplementation = type
        };
    }
}

``` 


### src > ImanSoftware.Framework > FrameworkDependencyInjection > ImanFrameworkDependencyInjectionOptions 

```csharp 
﻿using ImanSoftware.Caching;
using ImanSoftware.DateTimeProvider;
using ImanSoftware.FileStorage;
using ImanSoftware.GuidGenerator;
using ImanSoftware.Jwt;
using ImanSoftware.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace ImanSoftware.Framework.FrameworkDependencyInjection;

public sealed class ImanFrameworkDependencyInjectionOptions
{
    #region DateTimeProvider
    public bool AddDateTimeProvider { get; set; } = true;
    public ImanFrameworkDependencyInjectionCustomImplementation<IDateTimeProvider>? DateTimeProviderCustomImplementation {  get; set; }
    #endregion


    #region GuidGenerator
    public bool AddGuidGenerator { get; set; } = true;
    public GuidGeneratorDefaultImplementationType GuidGeneratorImplementationType { get; set; } = GuidGeneratorDefaultImplementationType.Sequential;
    public ImanFrameworkDependencyInjectionCustomImplementation<IGuidGenerator>? GuidGeneratorCustomImplementation { get; set; }
    #endregion


    #region CurrentUserAccessor
    public bool AddCurrentUserAccessor { get; set; } = true;
    public Action<IServiceCollection>? CurrentUserAccessorCustomRegistration { get; set; }
    #endregion


    #region JsonSerializer
    public bool AddJsonSerializer { get; set; } = true;
    public SerializationDefaultImplementationType SerializationImplementationType { get; set; } = SerializationDefaultImplementationType.Newtonsoft;
    public ImanFrameworkDependencyInjectionCustomImplementation<IImanJsonSerializer>? JsonSerializerCustomImplementation { get; set; }
    #endregion


    #region CacheService
    public bool AddCacheService { get; set; } = false;
    public ImanFrameworkDependencyInjectionCustomImplementation<ICacheService>? CacheServiceCustomImplementation { get; set; }
    #endregion


    #region Jwt
    public bool AddJwtService { get; set; } = false; 
    public JwtOptions? JwtConfigureOptions { get; set; }
    public ImanFrameworkDependencyInjectionCustomImplementation<IJwtTokenService>? JwtCustomImplementation { get; set; }
    #endregion


    #region DataAccess
    public bool AddDataAccess { get; set; } 
    public string? DataAccessConnectionString { get; set; }
    public Action<IServiceCollection>? DataAccessCustomRegistration { get; set; }
    #endregion


    #region DataAccess.Dapper
    public bool AddDapperDataAccess { get; set; } 
    public string? DapperConnectionString { get; set; }
    #endregion


    #region FileStorage
    public bool AddFileStorage { get; set; } 
    public Action<LocalFileStorageOptions>? FileStorageConfigureOptions { get; set; }
    public Action<IServiceCollection>? FileStorageCustomRegistration { get; set; }
    #endregion
}

``` 


### src > ImanSoftware.GuidGenerator > ImanSoftware.GuidGenerator 

```xml 
﻿<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" />
  </ItemGroup>

  <PropertyGroup>
    <PackageId>ImanSoftware.GuidGenerator</PackageId>
    <Version>1.0.0</Version>
    <Description>Generates sequential (database-friendly) and random GUIDs with dependency injection support.</Description>
    <PackageTags>guid;sequential;uuid;generator;database</PackageTags>
  </PropertyGroup>
  
</Project>

``` 


### src > ImanSoftware.GuidGenerator > Abstractions > GuidGeneratorImpelementationType 

```csharp 
﻿
namespace ImanSoftware.GuidGenerator;

public enum GuidGeneratorDefaultImplementationType
{
    Random,
    Sequential,
}

``` 


### src > ImanSoftware.GuidGenerator > Abstractions > GuidGeneratorRegistration 

```csharp 
﻿using ImanSoftware.Framework.Infrastructure.GuidGenerator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ImanSoftware.GuidGenerator;

public static class GuidGeneratorRegistration
{
    public static IServiceCollection AddGuidGenerator(
    this IServiceCollection services,
    GuidGeneratorDefaultImplementationType ImplementationType = GuidGeneratorDefaultImplementationType.Sequential,
    Type? customImplementation = null)
    {
        if (customImplementation is not null)
            services.AddSingleton(typeof(IGuidGenerator), customImplementation);
        else
        {
            switch (ImplementationType)
            {
                case GuidGeneratorDefaultImplementationType.Random:
                    services.TryAddSingleton<IGuidGenerator, RandomGuidGenerator>();
                    break;
                case GuidGeneratorDefaultImplementationType.Sequential:
                    services.TryAddSingleton<IGuidGenerator, SequentialGuidGenerator>();
                    break;
                default:
                    services.TryAddSingleton<IGuidGenerator, SequentialGuidGenerator>();
                    break;
            }

        }

        return services;
    }
}

``` 


### src > ImanSoftware.GuidGenerator > Abstractions > IGuidGenerator 

```csharp 
﻿
namespace ImanSoftware.GuidGenerator;

/// <summary>
/// Contract for generating unique identifiers. 
/// Uses SequentialGuid to prevent database index fragmentation.
/// </summary>
public interface IGuidGenerator
{
    /// <summary>
    /// Generates a new unique identifier (Guid) based on the configured strategy.
    /// </summary>
    Guid NewGuid();
}

``` 


### src > ImanSoftware.GuidGenerator > Infrastructure > RandomGuidGenerator 

```csharp 
﻿using ImanSoftware.GuidGenerator;

namespace ImanSoftware.Framework.Infrastructure.GuidGenerator;

internal class RandomGuidGenerator : IGuidGenerator
{
    public Guid NewGuid()
        => Guid.NewGuid();
}

``` 


### src > ImanSoftware.GuidGenerator > Infrastructure > SequentialGuidGenerator 

```csharp 
﻿using ImanSoftware.GuidGenerator;

namespace ImanSoftware.Framework.Infrastructure.GuidGenerator;

/// <summary>
/// Sequential Guid generator that creates GUIDs in chronological order 
/// to prevent database index fragmentation and improve performance.
/// </summary>
public class SequentialGuidGenerator : IGuidGenerator
{
    public Guid NewGuid()
    {
        byte[] guidArray = Guid.NewGuid().ToByteArray();
        DateTime now = DateTime.Now;

        byte[] dateTimeBytes = BitConverter.GetBytes(now.Ticks / 10000L);

        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(dateTimeBytes);
        }

        Array.Copy(dateTimeBytes, dateTimeBytes.Length - 6, guidArray, guidArray.Length - 6, 6);

        return new Guid(guidArray);
    }
}

``` 


### src > ImanSoftware.Jwt > ImanSoftware.Jwt 

```xml 
﻿<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Http" />
    <PackageReference Include="Microsoft.IdentityModel.Tokens" />
    <PackageReference Include="System.IdentityModel.Tokens.Jwt" />
  </ItemGroup>


  <PropertyGroup>
    <PackageId>ImanSoftware.Jwt</PackageId>
    <Version>1.0.0</Version>
    <Description>JWT token generation, validation, and authentication middleware for ASP.NET Core.</Description>
    <PackageTags>jwt;authentication;tokens;security;middleware</PackageTags>
  </PropertyGroup>


</Project>

``` 


### src > ImanSoftware.Jwt > Abstractions > IJwtTokenService 

```csharp 
﻿
using System.Security.Claims;

namespace ImanSoftware.Jwt;

public interface IJwtTokenService
{
    string GenerateToken(IEnumerable<Claim> claims);
    JwtTokenValidationResult ValidateToken(string token); // ارجاع به Core
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
``` 


### src > ImanSoftware.Jwt > Abstractions > JwtTokenServiceRegistration 

```csharp 
﻿using Microsoft.Extensions.DependencyInjection;

namespace ImanSoftware.Jwt;

public static class JwtTokenServiceRegistration
{
    public static IServiceCollection AddJwtTokenService(
        this IServiceCollection services,
        JwtOptions? configureOptions,
        Type? customImplementation = null)
    {
        if (customImplementation is not null)
        {
            services.AddSingleton(typeof(IJwtTokenService), customImplementation);
        }
        else
        {
            ArgumentNullException.ThrowIfNull(configureOptions, nameof(configureOptions));
            services.Configure<JwtOptions>(options =>
            {
                options.SecretKey = configureOptions.SecretKey;
                options.Issuer = configureOptions.Issuer;
                options.Audience = configureOptions.Audience;
                options.ExpiryMinutes = configureOptions.ExpiryMinutes;
                options.ValidateLifetime = configureOptions.ValidateLifetime;
            });
            services.AddSingleton<IJwtTokenService, JwtTokenService>();
        }

        return services;
    }
}
``` 


### src > ImanSoftware.Jwt > Core > JwtTokenValidationResult 

```csharp 
﻿using System.Security.Claims;

namespace ImanSoftware.Jwt;

/// <summary>
/// Represents the result of validating a JWT token.
/// </summary>
public class JwtTokenValidationResult
{
    public bool IsValid { get; }
    public ClaimsPrincipal? Principal { get; }
    public string? FailureReason { get; }

    private JwtTokenValidationResult(bool isValid, ClaimsPrincipal? principal, string? failureReason)
    {
        IsValid = isValid;
        Principal = principal;
        FailureReason = failureReason;
    }

    public static JwtTokenValidationResult Success(ClaimsPrincipal principal)
        => new(true, principal, null);

    public static JwtTokenValidationResult Failure(string reason)
        => new(false, null, reason);
}
``` 


### src > ImanSoftware.Jwt > Infrastructure > JwtOptions 

```csharp 
﻿namespace ImanSoftware.Jwt;

/// <summary>
/// Configuration options for JWT authentication.
/// </summary>
public class JwtOptions
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; } = 60;
    public bool ValidateLifetime { get; set; } = true;
}
``` 


### src > ImanSoftware.Jwt > Infrastructure > JwtTokenService 

```csharp 
﻿// Infrastructure/Jwt/JwtTokenService.cs
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ImanSoftware.Jwt;

/// <summary>
/// Default implementation of IJwtTokenService using System.IdentityModel.Tokens.Jwt.
/// </summary>
public class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _options;
    private readonly TokenValidationParameters _validationParameters;

    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
        _validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _options.Issuer,
            ValidateAudience = true,
            ValidAudience = _options.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
            ValidateLifetime = _options.ValidateLifetime,
            ClockSkew = TimeSpan.Zero
        };
    }

    public string GenerateToken(IEnumerable<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_options.ExpiryMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public JwtTokenValidationResult ValidateToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(token, _validationParameters, out _);
            return JwtTokenValidationResult.Success(principal);
        }
        catch (Exception ex)
        {
            return JwtTokenValidationResult.Failure(ex.Message);
        }
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _options.Issuer,
                ValidateAudience = true,
                ValidAudience = _options.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
                ValidateLifetime = false 
            };

            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(token, validationParameters, out _);
            return principal;
        }
        catch
        {
            return null;
        }
    }
}
``` 


### src > ImanSoftware.Jwt > Middlewares > JwtMiddleware 

```csharp 
﻿using ImanSoftware.Jwt;
using Microsoft.AspNetCore.Http;

namespace ImanSoftware.AspNetCore.Middlewares;

/// <summary>
/// Middleware to extract JWT token from Authorization header and set the user in HttpContext.
/// </summary>
public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IJwtTokenService _tokenService;

    public JwtMiddleware(RequestDelegate next, IJwtTokenService tokenService)
    {
        _next = next;
        _tokenService = tokenService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var token = ExtractTokenFromHeader(context);
        if (!string.IsNullOrEmpty(token))
        {
            var validationResult = _tokenService.ValidateToken(token);
            if (validationResult.IsValid && validationResult.Principal is not null)
            {
                context.User = validationResult.Principal;
            }
        }

        await _next(context);
    }

    private static string? ExtractTokenFromHeader(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            var parts = authHeader.ToString().Split(' ');
            if (parts.Length == 2 && parts[0].Equals("Bearer", StringComparison.OrdinalIgnoreCase))
                return parts[1];
        }
        return null;
    }
}
``` 


### src > ImanSoftware.Jwt > Middlewares > UseMiddlewareExtensions 

```csharp 
﻿using ImanSoftware.Jwt;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ImanSoftware.AspNetCore.Middlewares;

public static class UseMiddlewareExtensions
{
    public static IApplicationBuilder UseJwtAuthentication(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var tokenService = scope.ServiceProvider.GetService<IJwtTokenService>();

        if (tokenService is null)
        {
            throw new InvalidOperationException("IJwtTokenService is not registered in the DI container. ");
        }

        return app.UseMiddleware<JwtMiddleware>();
    }
}

``` 


### src > ImanSoftware.Logging > ImanSoftware.Logging 

```xml 
﻿<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.Logging.Abstractions" />
  </ItemGroup>
  
  <PropertyGroup>
    <PackageId>ImanSoftware.Logging</PackageId>
    <Version>1.0.1</Version>
    <Description>Structured logger adapter with ILogger abstraction, supporting scopes and standard log levels.</Description>
    <PackageTags>logging;logger;abstraction;structured-logging;diagnostics</PackageTags>
  </PropertyGroup>
  
</Project>

``` 


### src > ImanSoftware.Logging > Abstractions > ILoggerAdapter 

```csharp 
﻿using Microsoft.Extensions.Logging;

namespace ImanSoftware.Logging;

/// <summary>
/// Contract for a structured logger adapter.
/// Provides a unified abstraction over Microsoft.Extensions.Logging and other logging frameworks.
/// </summary>
/// <typeparam name="TCategoryName">The type whose name is used for the logger category name.</typeparam>
public interface ILoggerAdapter<out TCategoryName>
{
    /// <summary>
    /// Gets the category name of the logger.
    /// </summary>
    string CategoryName { get; }

    /// <summary>
    /// Checks if the given log level is enabled.
    /// </summary>
    bool IsEnabled(LogLevel logLevel);

    /// <summary>
    /// Logs a message with Information level.
    /// </summary>
    void LogInformation(string message, params object?[] args);

    /// <summary>
    /// Logs a message with Warning level.
    /// </summary>
    void LogWarning(string message, params object?[] args);

    /// <summary>
    /// Logs a message with Error level.
    /// </summary>
    void LogError(string message, params object?[] args);

    /// <summary>
    /// Logs an exception with Error level.
    /// </summary>
    void LogError(Exception exception, string message, params object?[] args);

    /// <summary>
    /// Logs a message with Debug level.
    /// </summary>
    void LogDebug(string message, params object?[] args);

    /// <summary>
    /// Logs a message with Trace level.
    /// </summary>
    void LogTrace(string message, params object?[] args);

    /// <summary>
    /// Logs a message with Critical level.
    /// </summary>
    void LogCritical(string message, params object?[] args);

    /// <summary>
    /// Logs a message with the specified log level.
    /// </summary>
    void Log(LogLevel logLevel, string message, params object?[] args);

    /// <summary>
    /// Begins a logical operation scope.
    /// </summary>
    IDisposable? BeginScope<TState>(TState state) where TState : notnull;
}
``` 


### src > ImanSoftware.Logging > Abstractions > LoggerAdapterRegistration 

```csharp 
﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ImanSoftware.Logging;

public static class LoggerAdapterRegistration
{
    public static IServiceCollection AddLoggerAdapter(
        this IServiceCollection services,
        Action<IServiceCollection>? customRegistration = null)
    {
        if (customRegistration is not null)
        {
            customRegistration(services);
        }
        else
        {
            services.TryAddSingleton(typeof(ILoggerAdapter<>), typeof(LoggerAdapter<>));
        }

        return services;
    }
}
``` 


### src > ImanSoftware.Logging > Infrastructure > LoggerAdapter 

```csharp 
﻿using Microsoft.Extensions.Logging;

namespace ImanSoftware.Logging;

/// <summary>
/// Default implementation of ILoggerAdapter using Microsoft.Extensions.Logging.ILogger.
/// </summary>
public class LoggerAdapter<TCategoryName> : ILoggerAdapter<TCategoryName>
{
    private readonly ILogger _logger;

    public string CategoryName => typeof(TCategoryName).FullName ?? typeof(TCategoryName).Name;

    public LoggerAdapter(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger(CategoryName);
    }

    public bool IsEnabled(LogLevel logLevel) => _logger.IsEnabled(logLevel);

    public void LogInformation(string message, params object?[] args)
    {
        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation(message, args);
    }

    public void LogWarning(string message, params object?[] args)
    {
        if (_logger.IsEnabled(LogLevel.Warning))
            _logger.LogWarning(message, args);
    }

    public void LogError(string message, params object?[] args)
    {
        if (_logger.IsEnabled(LogLevel.Error))
            _logger.LogError(message, args);
    }

    public void LogError(Exception exception, string message, params object?[] args)
    {
        if (_logger.IsEnabled(LogLevel.Error))
            _logger.LogError(exception, message, args);
    }

    public void LogDebug(string message, params object?[] args)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
            _logger.LogDebug(message, args);
    }

    public void LogTrace(string message, params object?[] args)
    {
        if (_logger.IsEnabled(LogLevel.Trace))
            _logger.LogTrace(message, args);
    }

    public void LogCritical(string message, params object?[] args)
    {
        if (_logger.IsEnabled(LogLevel.Critical))
            _logger.LogCritical(message, args);
    }

    public void Log(LogLevel logLevel, string message, params object?[] args)
    {
        if (_logger.IsEnabled(logLevel))
            _logger.Log(logLevel, message, args);
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return _logger.BeginScope(state);
    }
}
``` 


### src > ImanSoftware.Outcome > ImanSoftware.Outcome 

```xml 
﻿<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <PropertyGroup>
    <PackageId>ImanSoftware.Outcome</PackageId>
    <Version>1.0.3</Version>
    <Description>A lightweight Result/Outcome pattern implementation for .NET — consistent error handling without exceptions.</Description>
    <PackageTags>result-pattern;outcome;error-handling;core;result</PackageTags>
  </PropertyGroup>
  
</Project>

``` 


### src > ImanSoftware.Outcome > Exceptions > BadRequestException 

```csharp 
﻿


namespace ImanSoftware.Outcomes;

/// <summary>
/// Exception thrown for general bad requests (HTTP 400).
/// </summary>
public class BadRequestException : FrameworkException
{
    public BadRequestException(string message)
        : base(message, "BAD_REQUEST", OutcomeErrorType.Failure)
    {
    }

    public BadRequestException(string message, Exception innerException)
        : base(message, "BAD_REQUEST", OutcomeErrorType.Failure, innerException)
    {
    }
}
``` 


### src > ImanSoftware.Outcome > Exceptions > BusinessRuleException 

```csharp 
﻿


namespace ImanSoftware.Outcomes;

/// <summary>
/// Exception thrown when a business rule is violated (HTTP 422 or 400).
/// </summary>
public class BusinessRuleException : FrameworkException
{
    public BusinessRuleException(string message)
        : base(message, "BUSINESS_RULE_VIOLATION", OutcomeErrorType.Failure)
    {
    }

    public BusinessRuleException(string message, Exception innerException)
        : base(message, "BUSINESS_RULE_VIOLATION", OutcomeErrorType.Failure, innerException)
    {
    }
}
``` 


### src > ImanSoftware.Outcome > Exceptions > ConflictException 

```csharp 
﻿


namespace ImanSoftware.Outcomes;

/// <summary>
/// Exception thrown when a conflict occurs (HTTP 409).
/// </summary>
public class ConflictException : FrameworkException
{
    public ConflictException(string message)
        : base(message, "CONFLICT", OutcomeErrorType.Conflict)
    {
    }

    public ConflictException(string message, Exception innerException)
        : base(message, "CONFLICT", OutcomeErrorType.Conflict, innerException)
    {
    }
}
``` 


### src > ImanSoftware.Outcome > Exceptions > ForbiddenException 

```csharp 
﻿
using System;
using System.Collections.Generic;
using System.Text;

namespace ImanSoftware.Outcomes;

/// <summary>
/// Exception thrown when the user is authenticated but not authorized (HTTP 403).
/// </summary>
public class ForbiddenException : FrameworkException
{
    public ForbiddenException(string message = "You do not have permission to access this resource.")
        : base(message, "FORBIDDEN", OutcomeErrorType.Failure)
    {
    }

    public ForbiddenException(string message, Exception innerException)
        : base(message, "FORBIDDEN", OutcomeErrorType.Failure, innerException)
    {
    }
}
``` 


### src > ImanSoftware.Outcome > Exceptions > FrameworkException 

```csharp 
﻿

namespace ImanSoftware.Outcomes;

public abstract class FrameworkException : Exception
{
    public string ErrorCode { get; }
    public OutcomeErrorType Type { get; }

    protected FrameworkException(string message, string errorCode, OutcomeErrorType errorType)
        : base(message)
    {
        ErrorCode = errorCode;
        Type = errorType;
    }

    protected FrameworkException(string message, string errorCode, OutcomeErrorType errorType, Exception innerException)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
        Type = errorType;
    }

    /// <summary>
    /// Converts the exception to a Result object (without data).
    /// </summary>
    public Outcome ToResult()
    {
        return Outcome.Failure(new OutcomeError(Message, ErrorCode, Type));
    }

    /// <summary>
    /// Converts the exception to a Result object with data.
    /// </summary>
    public Outcome<T> ToResult<T>()
    {
        return Outcome.Failure<T>(new OutcomeError(Message, ErrorCode, Type));
    }
}
``` 


### src > ImanSoftware.Outcome > Exceptions > NotFoundException 

```csharp 
﻿
using System;
using System.Collections.Generic;
using System.Text;

namespace ImanSoftware.Outcomes;

/// <summary>
/// Exception thrown when a requested resource is not found (HTTP 404 equivalent).
/// </summary>
public class NotFoundException : FrameworkException
{
    public NotFoundException(string message)
        : base(message, "NOT_FOUND", OutcomeErrorType.NotFound)
    {
    }

    public NotFoundException(string message, Exception innerException)
        : base(message, "NOT_FOUND", OutcomeErrorType.NotFound, innerException)
    {
    }
}
``` 


### src > ImanSoftware.Outcome > Exceptions > UnauthorizedException 

```csharp 
﻿


namespace ImanSoftware.Outcomes;

/// <summary>
/// Exception thrown when the user is not authenticated (HTTP 401).
/// </summary>
public class UnauthorizedException : FrameworkException
{
    public UnauthorizedException(string message = "User is not authenticated.")
        : base(message, "UNAUTHORIZED", OutcomeErrorType.Failure)
    {
    }

    public UnauthorizedException(string message, Exception innerException)
        : base(message, "UNAUTHORIZED", OutcomeErrorType.Failure, innerException)
    {
    }
}
``` 


### src > ImanSoftware.Outcome > Exceptions > ValidationException 

```csharp 
﻿
using System;
using System.Collections.Generic;
using System.Text;

namespace ImanSoftware.Outcomes;

/// <summary>
/// Exception thrown when validation fails (HTTP 400 with validation errors).
/// </summary>
public class ValidationException : FrameworkException
{
    public IReadOnlyCollection<string> Errors { get; }

    public ValidationException(string message, IReadOnlyCollection<string> errors)
        : base(message, "VALIDATION_ERROR", OutcomeErrorType.Validation)
    {
        Errors = errors;
    }

    public ValidationException(string message)
        : base(message, "VALIDATION_ERROR", OutcomeErrorType.Validation)
    {
        Errors = Array.Empty<string>();
    }

    public ValidationException(string message, Exception innerException)
        : base(message, "VALIDATION_ERROR", OutcomeErrorType.Validation, innerException)
    {
        Errors = Array.Empty<string>();
    }
}
``` 


### src > ImanSoftware.Outcome > Outcome > Outcome 

```csharp 
﻿
using System.Diagnostics.CodeAnalysis;

namespace ImanSoftware.Outcomes;

public class Outcome
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public OutcomeError Error { get; }

    public Outcome(bool isSuccess, OutcomeError error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Outcome Successful() => new(true, OutcomeError.None);
    public static Outcome<T> Successful<T>(T data) => new(data, true, OutcomeError.None);

    public static Outcome Failure(OutcomeError error) => new(false, error);
    public static Outcome<T> Failure<T>(OutcomeError error) => new(default, false, error);

}


public class Outcome<T> : Outcome
{
    private readonly T? _data;
    public Outcome(T? data, bool isSuccess, OutcomeError error) : base(isSuccess, error)
    {
        _data = data;
    }

    [NotNull]
    public T Data => IsSuccess && _data is not null ? _data! : throw new InvalidOperationException("Cannot access Data of a failed result. Check IsSuccess first.");


    public static implicit operator Outcome<T>(T? _data)
        => _data is not null ?
            Successful(_data) : Failure<T>(OutcomeError.Create("", "", OutcomeErrorType.Problem));
}

``` 


### src > ImanSoftware.Outcome > Outcome > OutcomeError 

```csharp 
﻿

namespace ImanSoftware.Outcomes;

public record OutcomeError
{
    public string Message { get; }
    public string Code { get; } = string.Empty;
    public OutcomeErrorType Type { get; } = OutcomeErrorType.Failure;

    public OutcomeError(string message, string code, OutcomeErrorType type)
    {
        Message = message;
        Code = code;
        Type = type;
    }

    public OutcomeError(Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        Message = exception.Message;
        Code = string.Empty;
    }

    public static OutcomeError DefaultError => new("Somthing went Wrong . . .", "", OutcomeErrorType.Failure);
    public static OutcomeError None => new(string.Empty, string.Empty, OutcomeErrorType.None);
    public static OutcomeError Create(string msg, string code, OutcomeErrorType type) => new(msg, code, type);
}

``` 


### src > ImanSoftware.Outcome > Outcome > OutcomeErrorType 

```csharp 
﻿

namespace ImanSoftware.Outcomes;

public enum OutcomeErrorType
{
    None = 0,
    Failure = 1,
    Validation = 2,
    Problem = 3,
    NotFound = 4,
    Conflict = 5,
    InquiryError = 6
}

``` 


### src > ImanSoftware.RateLimiter > ImanSoftware.RateLimiter 

```xml 
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <!-- Framework dependencies -->
    <FrameworkReference Include="Microsoft.AspNetCore.App" />
    <PackageReference Include="ImanSoftware.OutCome" />
    <PackageReference Include="ImanSoftware.Logging" />
    <PackageReference Include="ImanSoftware.Extensions" />
  </ItemGroup>
  <PropertyGroup>
    <PackageId>ImanSoftware.RateLimiter</PackageId>
    <Version>1.0.6</Version>
    <Description>Rate limiting abstraction with in-memory and distributed support.</Description>
    <PackageTags>ratelimiter;throttling;fixedwindow;tokenbucket;dotnet</PackageTags>
  </PropertyGroup>
</Project>
``` 


### src > ImanSoftware.RateLimiter > Abstractions > IRateLimiterMetrics 

```csharp 
﻿namespace ImanSoftware.RateLimiter.Abstractions;

/// <summary>
/// Contract for rate limiter metrics.
/// </summary>
public interface IRateLimiterMetrics
{
    /// <summary>
    /// Records an allowed request.
    /// </summary>
    void RecordAllowed(string policyName, string key);

    /// <summary>
    /// Records a rejected request.
    /// </summary>
    void RecordRejected(string policyName, string key, int retryAfterSeconds);
}
``` 


### src > ImanSoftware.RateLimiter > Abstractions > IRateLimiterPolicy 

```csharp 
﻿using ImanSoftware.RateLimiter.Core.Models;
using Microsoft.AspNetCore.Http;

namespace ImanSoftware.RateLimiter;

/// <summary>
/// Contract for custom rate limiter policies.
/// </summary>
public interface IRateLimiterPolicy
{
    /// <summary>
    /// Gets the partition key for the given HttpContext.
    /// </summary>
    string GetPartitionKey(HttpContext context);

    /// <summary>
    /// Gets the rate limit policy for the given context.
    /// </summary>
    RateLimitPolicy GetPolicy(HttpContext context);
}
``` 


### src > ImanSoftware.RateLimiter > Attributes > EnableImanRateLimitingAttribute 

```csharp 
﻿using ImanSoftware.Logging;
using ImanSoftware.RateLimiter.Core.Enums;
using ImanSoftware.RateLimiter.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.RateLimiting;

namespace ImanSoftware.RateLimiter.Attributes;

/// <summary>
/// Attribute to enable rate limiting on a controller or endpoint.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class EnableImanRateLimitingAttribute : Attribute, IFilterFactory
{
    public string PolicyName { get; set; } = "Default";

    public bool IsReusable => false;

    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        var options = serviceProvider.GetRequiredService<ImanRateLimiterOptions>();
        var logger = serviceProvider.GetService<ILoggerAdapter<EnableImanRateLimitingFilter>>();

        return new EnableImanRateLimitingFilter(PolicyName, options, logger);
    }
}

/// <summary>
/// Filter implementation for rate limiting.
/// </summary>
public class EnableImanRateLimitingFilter : IAsyncActionFilter
{
    private readonly string _policyName;
    private readonly ImanRateLimiterOptions _options;
    private readonly ILoggerAdapter<EnableImanRateLimitingFilter>? _logger;

    public EnableImanRateLimitingFilter(
        string policyName,
        ImanRateLimiterOptions options,
        ILoggerAdapter<EnableImanRateLimitingFilter>? logger)
    {
        _policyName = policyName;
        _options = options;
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Check if the policy exists
        var policy = _policyName == "Default"
            ? _options.DefaultPolicy
            : _options.Policies.GetValueOrDefault(_policyName);

        if (policy is null)
        {
            _logger?.LogWarning("Rate limit policy '{PolicyName}' not found. Using default.", _policyName);
            policy = _options.DefaultPolicy;
        }

        var httpContext = context.HttpContext;
        var limiter = GetLimiterForPolicy(_policyName, policy);

        var lease = await limiter.AcquireAsync(httpContext, 1);

        if (lease is not null && !lease.IsAcquired)
        {
            // دریافت RetryAfter از Metadata
            TimeSpan? retryAfter = null;
            if (lease.TryGetMetadata(MetadataName.RetryAfter, out TimeSpan retryAfterValue))
            {
                retryAfter = retryAfterValue;
            }

            var retryAfterSeconds = (int)(retryAfter?.TotalSeconds ?? 60);

            // تنظیم هدرها در پاسخ رد شده
            if (policy.IncludeHeaders)
            {
                httpContext.Response.Headers["X-RateLimit-Limit"] = policy.PermitLimit.ToString();
                httpContext.Response.Headers["X-RateLimit-Remaining"] = "0";
                httpContext.Response.Headers["Retry-After"] = retryAfterSeconds.ToString();
            }

            context.Result = new ObjectResult(new
            {
                Error = "Rate limit exceeded",
                RetryAfter = retryAfterSeconds,
                Policy = _policyName,
                Limit = policy.PermitLimit,
                Window = policy.Window.TotalSeconds
            })
            {
                StatusCode = StatusCodes.Status429TooManyRequests
            };

            return;
        }

        // Request allowed - add remaining count if available
        if (policy.IncludeHeaders && lease is not null)
        {
            // ✅ دریافت تعداد مجوزهای باقی‌مانده از متادیتا
            int? remaining = GetRemainingTokens(lease,policy);

            var remainingAfter = (remaining ?? policy.PermitLimit) - 1;
            httpContext.Response.Headers["X-RateLimit-Remaining"] = Math.Max(0, remainingAfter).ToString();
            httpContext.Response.Headers["X-RateLimit-Limit"] = policy.PermitLimit.ToString();
            httpContext.Response.Headers["X-RateLimit-Policy"] = policy.Strategy.ToString();
        }

        await next();
    }

    private static string GetPartitionKey(HttpContext context, RateLimitPolicy policy)
    {
        return policy.PartitionBy switch
        {
            RateLimitPartitionBy.IpAddress => context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            RateLimitPartitionBy.UserId => context.User.FindFirst("sub")?.Value
                                          ?? context.User.FindFirst("nameid")?.Value
                                          ?? "anonymous",
            RateLimitPartitionBy.ApiKey => context.Request.Headers["X-API-Key"].FirstOrDefault() ?? "unknown",
            RateLimitPartitionBy.Endpoint => context.Request.Path.ToString(),
            RateLimitPartitionBy.Custom => policy.CustomPartitionResolver?.Invoke(context) ?? "custom",
            _ => "default"
        };
    }

    private static PartitionedRateLimiter<HttpContext> GetLimiterForPolicy(
        string policyName,
        RateLimitPolicy policy)
    {
        return PartitionedRateLimiter.Create<HttpContext, string>(
            context =>
            {
                var partitionKey = GetPartitionKey(context, policy);
                return policy.Strategy switch
                {
                    RateLimiterStrategy.FixedWindow => RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey,
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = policy.PermitLimit,
                            Window = policy.Window,
                            QueueLimit = policy.QueueLimit ?? 0
                        }),
                    RateLimiterStrategy.SlidingWindow => RateLimitPartition.GetSlidingWindowLimiter(
                        partitionKey,
                        _ => new SlidingWindowRateLimiterOptions
                        {
                            PermitLimit = policy.PermitLimit,
                            Window = policy.Window,
                            SegmentsPerWindow = 10,
                            QueueLimit = policy.QueueLimit ?? 0
                        }),
                    RateLimiterStrategy.TokenBucket => RateLimitPartition.GetTokenBucketLimiter(
                        partitionKey,
                        _ => new TokenBucketRateLimiterOptions
                        {
                            TokenLimit = policy.PermitLimit,
                            QueueLimit = policy.QueueLimit ?? 0,
                            TokensPerPeriod = policy.TokensPerSecond ?? 10,
                            AutoReplenishment = true,
                            ReplenishmentPeriod = TimeSpan.FromSeconds(1)
                        }),
                    RateLimiterStrategy.Concurrency => RateLimitPartition.GetConcurrencyLimiter(
                        partitionKey,
                        _ => new ConcurrencyLimiterOptions
                        {
                            PermitLimit = policy.PermitLimit,
                            QueueLimit = policy.QueueLimit ?? 0
                        }),
                    _ => RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey,
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = policy.PermitLimit,
                            Window = policy.Window
                        })
                };
            });
    }

    /// <summary>
    /// Tries to get remaining tokens from the lease metadata using common keys.
    /// </summary>
    private static int? GetRemainingTokens(RateLimitLease lease, RateLimitPolicy policy)
    {
        // ابتدا سعی می‌کنیم متادیتا را بگیریم
        if (lease.TryGetMetadata("TotalTokens", out object? totalTokensObj) && totalTokensObj is int totalTokens)
            return totalTokens;

        // اگر نداشت، از استراتژی استفاده می‌کنیم
        return policy.Strategy switch
        {
            RateLimiterStrategy.TokenBucket or RateLimiterStrategy.Concurrency => policy.PermitLimit,
            _ => null
        };
    }
}
``` 


### src > ImanSoftware.RateLimiter > Core > Enums > RateLimiterStrategy 

```csharp 
﻿namespace ImanSoftware.RateLimiter.Core.Enums;

/// <summary>
/// Defines the rate limiting strategy algorithms.
/// </summary>
public enum RateLimiterStrategy
{
    /// <summary>
    /// Fixed window strategy: limits requests within a fixed time window.
    /// </summary>
    FixedWindow,

    /// <summary>
    /// Sliding window strategy: limits requests within a sliding time window.
    /// </summary>
    SlidingWindow,

    /// <summary>
    /// Token bucket strategy: tokens are refilled at a fixed rate.
    /// </summary>
    TokenBucket,

    /// <summary>
    /// Concurrency strategy: limits the number of concurrent requests.
    /// </summary>
    Concurrency
}
``` 


### src > ImanSoftware.RateLimiter > Core > Enums > RateLimitPartitionBy 

```csharp 
﻿namespace ImanSoftware.RateLimiter.Core.Enums;

/// <summary>
/// Defines the partition key for rate limiting.
/// </summary>
public enum RateLimitPartitionBy
{
    /// <summary>
    /// Partition by client IP address.
    /// </summary>
    IpAddress,

    /// <summary>
    /// Partition by authenticated user ID.
    /// </summary>
    UserId,

    /// <summary>
    /// Partition by API key.
    /// </summary>
    ApiKey,

    /// <summary>
    /// Partition by endpoint path.
    /// </summary>
    Endpoint,

    /// <summary>
    /// Custom partition key (user-defined).
    /// </summary>
    Custom
}
``` 


### src > ImanSoftware.RateLimiter > Core > Models > ImanRateLimiterOptions 

```csharp 
﻿using Microsoft.AspNetCore.Http;

namespace ImanSoftware.RateLimiter.Core.Models;

/// <summary>
/// Configuration options for the ImanSoftware Rate Limiter.
/// </summary>
public class ImanRateLimiterOptions
{
    /// <summary>
    /// Default policy applied to all endpoints.
    /// </summary>
    public RateLimitPolicy DefaultPolicy { get; set; } = new();

    /// <summary>
    /// Named policies for specific endpoints or controllers.
    /// </summary>
    public Dictionary<string, RateLimitPolicy> Policies { get; set; } = new();

    /// <summary>
    /// Whether to enable built-in metrics.
    /// </summary>
    public bool EnableMetrics { get; set; } = true;

    /// <summary>
    /// Whether to enable logging for rate limit events.
    /// </summary>
    public bool EnableLogging { get; set; } = true;

    /// <summary>
    /// Custom logging handler.
    /// </summary>
    public Action<RateLimitEvent>? LogHandler { get; set; }

    /// <summary>
    /// Custom on-rejected handler.
    /// </summary>
    public Func<RateLimitRejectedContext, Task>? OnRejected { get; set; }
}

/// <summary>
/// Rate limit event for logging.
/// </summary>
public record RateLimitEvent
{
    public string Key { get; init; } = string.Empty;
    public bool IsAllowed { get; init; }
    public int Remaining { get; init; }
    public int RetryAfterSeconds { get; init; }
    public string PolicyName { get; init; } = "Default";
    public DateTimeOffset Timestamp { get; init; }
}

/// <summary>
/// Context for on-rejected callback.
/// </summary>
public record RateLimitRejectedContext
{
    public HttpContext HttpContext { get; init; } = default!;
    public string Key { get; init; } = string.Empty;
    public int RetryAfterSeconds { get; init; }
    public RateLimitPolicy Policy { get; init; } = new();
}
``` 


### src > ImanSoftware.RateLimiter > Core > Models > RateLimitHeaders 

```csharp 
﻿using Microsoft.AspNetCore.Http;

namespace ImanSoftware.RateLimiter.Core.Models;

/// <summary>
/// Standard rate limit response headers.
/// </summary>
public record RateLimitHeaders
{
    public int Limit { get; init; }
    public int Remaining { get; init; }
    public DateTimeOffset Reset { get; init; }

    public void ApplyTo(HttpResponse response)
    {
        response.Headers["X-RateLimit-Limit"] = Limit.ToString();
        response.Headers["X-RateLimit-Remaining"] = Remaining.ToString();
        response.Headers["X-RateLimit-Reset"] = Reset.ToUnixTimeSeconds().ToString();
    }
}
``` 


### src > ImanSoftware.RateLimiter > Core > Models > RateLimitPolicy 

```csharp 
﻿using ImanSoftware.RateLimiter.Core.Enums;
using Microsoft.AspNetCore.Http;

namespace ImanSoftware.RateLimiter.Core.Models;

/// <summary>
/// Represents a rate limiting policy.
/// </summary>
public class RateLimitPolicy
{
    /// <summary>
    /// Maximum number of requests allowed within the window.
    /// </summary>
    public int PermitLimit { get; set; } = 100;

    /// <summary>
    /// Time window for the rate limit.
    /// </summary>
    public TimeSpan Window { get; set; } = TimeSpan.FromMinutes(1);

    /// <summary>
    /// The strategy algorithm to use.
    /// </summary>
    public RateLimiterStrategy Strategy { get; set; } = RateLimiterStrategy.FixedWindow;

    /// <summary>
    /// The partition key for this policy.
    /// </summary>
    public RateLimitPartitionBy PartitionBy { get; set; } = RateLimitPartitionBy.IpAddress;

    /// <summary>
    /// Custom partition resolver (used when PartitionBy is Custom).
    /// </summary>
    public Func<HttpContext, string>? CustomPartitionResolver { get; set; }

    /// <summary>
    /// Whether to include rate limit headers in the response.
    /// </summary>
    public bool IncludeHeaders { get; set; } = true;

    /// <summary>
    /// Queue limit for Token Bucket and Concurrency strategies.
    /// </summary>
    public int? QueueLimit { get; set; }

    /// <summary>
    /// Auto-refill rate for Token Bucket strategy (tokens per second).
    /// </summary>
    public int? TokensPerSecond { get; set; }
}
``` 


### src > ImanSoftware.RateLimiter > Infrastructure > DefaultRateLimiterPolicy 

```csharp 
﻿using ImanSoftware.RateLimiter.Core.Models;
using Microsoft.AspNetCore.Http;

namespace ImanSoftware.RateLimiter.Infrastructure;

/// <summary>
/// Default implementation of IRateLimiterPolicy.
/// </summary>
public class DefaultRateLimiterPolicy : IRateLimiterPolicy
{
    private readonly ImanRateLimiterOptions _options;

    public DefaultRateLimiterPolicy(ImanRateLimiterOptions options)
    {
        _options = options;
    }

    public string GetPartitionKey(HttpContext context)
    {
        var policy = _options.DefaultPolicy;
        return policy.PartitionBy switch
        {
            Core.Enums.RateLimitPartitionBy.IpAddress => context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            Core.Enums.RateLimitPartitionBy.UserId => context.User.FindFirst("sub")?.Value ?? "anonymous",
            Core.Enums.RateLimitPartitionBy.ApiKey => context.Request.Headers["X-API-Key"].FirstOrDefault() ?? "unknown",
            Core.Enums.RateLimitPartitionBy.Endpoint => context.Request.Path.ToString(),
            Core.Enums.RateLimitPartitionBy.Custom => policy.CustomPartitionResolver?.Invoke(context) ?? "custom",
            _ => "default"
        };
    }

    public RateLimitPolicy GetPolicy(HttpContext context)
    {
        return _options.DefaultPolicy;
    }
}
``` 


### src > ImanSoftware.RateLimiter > Infrastructure > RateLimiterConfiguration 

```csharp 
﻿using ImanSoftware.RateLimiter.Core.Models;

namespace ImanSoftware.RateLimiter.Infrastructure;

/// <summary>
/// Configuration model for appsettings.json.
/// </summary>
public class RateLimiterConfiguration
{
    public RateLimitPolicy? DefaultPolicy { get; set; }
    public Dictionary<string, RateLimitPolicy> Policies { get; set; } = new();
    public bool EnableMetrics { get; set; } = true;
    public bool EnableLogging { get; set; } = true;
}
``` 


### src > ImanSoftware.RateLimiter > Infrastructure > RateLimiterMetrics 

```csharp 
﻿using System.Collections.Concurrent;
using ImanSoftware.RateLimiter.Abstractions;

namespace ImanSoftware.RateLimiter.Infrastructure;

/// <summary>
/// In-memory metrics implementation for rate limiter.
/// </summary>
public class RateLimiterMetrics : IRateLimiterMetrics
{
    private readonly ConcurrentDictionary<string, (long Allowed, long Rejected)> _counters = new();

    public void RecordAllowed(string policyName, string key)
    {
        var counter = _counters.GetOrAdd($"{policyName}:{key}", _ => (0, 0));
        _counters[$"{policyName}:{key}"] = (counter.Allowed + 1, counter.Rejected);
    }

    public void RecordRejected(string policyName, string key, int retryAfterSeconds)
    {
        var counter = _counters.GetOrAdd($"{policyName}:{key}", _ => (0, 0));
        _counters[$"{policyName}:{key}"] = (counter.Allowed, counter.Rejected + 1);
    }

    public (long Allowed, long Rejected) GetMetrics(string policyName, string key)
    {
        return _counters.GetValueOrDefault($"{policyName}:{key}", (0, 0));
    }
}
``` 


### src > ImanSoftware.RateLimiter > Infrastructure > RateLimiterServiceCollectionExtensions 

```csharp 
﻿using ImanSoftware.RateLimiter.Abstractions;
using ImanSoftware.RateLimiter.Core.Enums;
using ImanSoftware.RateLimiter.Core.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.RateLimiting;

namespace ImanSoftware.RateLimiter.Infrastructure;

/// <summary>
/// Extension methods for registering the ImanSoftware Rate Limiter.
/// </summary>
public static class RateLimiterServiceCollectionExtensions
{
    /// <summary>
    /// Adds the ImanSoftware Rate Limiter with default configuration.
    /// </summary>
    public static IServiceCollection AddImanRateLimiter(this IServiceCollection services)
        => services.AddImanRateLimiter(_ => { });

    /// <summary>
    /// Adds the ImanSoftware Rate Limiter with custom configuration.
    /// </summary>
    public static IServiceCollection AddImanRateLimiter(
        this IServiceCollection services,
        Action<ImanRateLimiterOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        var options = new ImanRateLimiterOptions();
        configure(options);

        services.AddSingleton(options);

        if (options.EnableMetrics)
            services.AddSingleton<IRateLimiterMetrics, RateLimiterMetrics>();

        services.AddSingleton<IRateLimiterPolicy, DefaultRateLimiterPolicy>();

        services.AddRateLimiter(rateLimiterOptions =>
        {
            // Global Limiter
            rateLimiterOptions.GlobalLimiter = CreateGlobalLimiter(options);

            // Named Policies
            foreach (var policy in options.Policies)
            {
                rateLimiterOptions.AddPolicy<string>(policy.Key, context =>
                    CreatePartitionedLimiter(context, policy.Value));
            }

            // Default Policy
            rateLimiterOptions.AddPolicy<string>("Default", context =>
                CreatePartitionedLimiter(context, options.DefaultPolicy));

            // OnRejected Callback
            if (options.OnRejected is not null)
            {
                rateLimiterOptions.OnRejected = async (context, token) =>
                {
                    var httpContext = context.HttpContext;

                    // ✅ دریافت RetryAfter از Metadata
                    TimeSpan? retryAfter = null;
                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out TimeSpan retryAfterValue))
                    {
                        retryAfter = retryAfterValue;
                    }

                    var retryAfterSeconds = (int)(retryAfter?.TotalSeconds ?? 60);

                    // دریافت Key (با فرض اینکه Key را قبلاً در GetPartitionKey محاسبه کرده‌اید)
                    var policy = options.DefaultPolicy; // یا از منطق مناسب برای انتخاب Policy استفاده کنید
                    var key = GetPartitionKey(httpContext, policy);

                    await options.OnRejected(new RateLimitRejectedContext
                    {
                        HttpContext = httpContext,
                        Key = key,
                        RetryAfterSeconds = retryAfterSeconds,
                        Policy = policy
                    });
                };
            }
        });

        return services;
    }

    /// <summary>
    /// Adds the ImanSoftware Rate Limiter from IConfiguration.
    /// </summary>
    public static IServiceCollection AddImanRateLimiter(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var config = configuration.Get<RateLimiterConfiguration>() ?? new RateLimiterConfiguration();

        return services.AddImanRateLimiter(options =>
        {
            options.DefaultPolicy = config.DefaultPolicy ?? new RateLimitPolicy();
            options.EnableMetrics = config.EnableMetrics;
            options.EnableLogging = config.EnableLogging;

            foreach (var policy in config.Policies)
                options.Policies[policy.Key] = policy.Value;
        });
    }

    // Private: Creates the global partitioned limiter
    private static PartitionedRateLimiter<HttpContext> CreateGlobalLimiter(ImanRateLimiterOptions options)
    {
        var policy = options.DefaultPolicy;
        return PartitionedRateLimiter.Create<HttpContext, string>(
            context =>
            {
                var partitionKey = GetPartitionKey(context, policy);
                return CreateLimiterPartition(partitionKey, policy);
            });
    }

    // Private: Creates a named policy limiter (simplified)
    private static RateLimitPartition<string> CreatePartitionedLimiter(
        HttpContext context,
        RateLimitPolicy policy)
    {
        var partitionKey = GetPartitionKey(context, policy);
        return CreateLimiterPartition(partitionKey, policy);
    }

    // Private: Creates a rate limit partition based on the policy strategy
    private static RateLimitPartition<string> CreateLimiterPartition(
        string partitionKey,
        RateLimitPolicy policy)
    {
        return policy.Strategy switch
        {
            RateLimiterStrategy.FixedWindow => RateLimitPartition.GetFixedWindowLimiter(
                partitionKey,
                _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = policy.PermitLimit,
                    Window = policy.Window,
                    QueueLimit = policy.QueueLimit ?? 0
                }),

            RateLimiterStrategy.SlidingWindow => RateLimitPartition.GetSlidingWindowLimiter(
                partitionKey,
                _ => new SlidingWindowRateLimiterOptions
                {
                    PermitLimit = policy.PermitLimit,
                    Window = policy.Window,
                    SegmentsPerWindow = 10,
                    QueueLimit = policy.QueueLimit ?? 0
                }),

            RateLimiterStrategy.TokenBucket => RateLimitPartition.GetTokenBucketLimiter(
                partitionKey,
                _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = policy.PermitLimit,
                    QueueLimit = policy.QueueLimit ?? 0,
                    TokensPerPeriod = policy.TokensPerSecond ?? 10,
                    AutoReplenishment = true,
                    ReplenishmentPeriod = TimeSpan.FromSeconds(1)
                }),

            RateLimiterStrategy.Concurrency => RateLimitPartition.GetConcurrencyLimiter(
                partitionKey,
                _ => new ConcurrencyLimiterOptions
                {
                    PermitLimit = policy.PermitLimit,
                    QueueLimit = policy.QueueLimit ?? 0
                }),

            _ => RateLimitPartition.GetFixedWindowLimiter(
                partitionKey,
                _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = policy.PermitLimit,
                    Window = policy.Window
                })
        };
    }

    // Private: Gets the partition key from HttpContext
    private static string GetPartitionKey(HttpContext context, RateLimitPolicy policy)
    {
        return policy.PartitionBy switch
        {
            RateLimitPartitionBy.IpAddress => context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            RateLimitPartitionBy.UserId => context.User.FindFirst("sub")?.Value
                                          ?? context.User.FindFirst("nameid")?.Value
                                          ?? "anonymous",
            RateLimitPartitionBy.ApiKey => context.Request.Headers["X-API-Key"].FirstOrDefault() ?? "unknown",
            RateLimitPartitionBy.Endpoint => context.Request.Path.ToString(),
            RateLimitPartitionBy.Custom => policy.CustomPartitionResolver?.Invoke(context) ?? "custom",
            _ => "default"
        };
    }
}
``` 


### src > ImanSoftware.RateLimiter > Middlewares > RateLimiterMiddleware 

```csharp 
﻿using ImanSoftware.Logging;
using ImanSoftware.RateLimiter.Core.Models;
using Microsoft.AspNetCore.Http;

namespace ImanSoftware.RateLimiter.Middlewares;

/// <summary>
/// Middleware for adding rate limit headers and logging.
/// </summary>
public class RateLimiterMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ImanRateLimiterOptions _options;
    private readonly ILoggerAdapter<RateLimiterMiddleware>? _logger;

    public RateLimiterMiddleware(
        RequestDelegate next,
        ImanRateLimiterOptions options,
        ILoggerAdapter<RateLimiterMiddleware>? logger = null)
    {
        _next = next;
        _options = options;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Store original response stream
        var originalBodyStream = context.Response.Body;

        // Use a memory stream to capture the response
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        // Continue pipeline
        await _next(context);

        // Check if rate limit headers should be added
        if (_options.DefaultPolicy.IncludeHeaders)
        {
            var headers = ExtractRateLimitHeaders(context);
            if (headers is not null)
            {
                headers.ApplyTo(context.Response);
                context.Response.Headers["X-RateLimit-Policy"] = _options.DefaultPolicy.Strategy.ToString();
            }
        }

        // Log the request
        if (_options.EnableLogging)
        {
            _logger?.LogInformation(
                "Request {Method} {Path} - Status: {StatusCode}, IP: {IP}",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                context.Connection.RemoteIpAddress?.ToString() ?? "unknown");
        }

        // Copy the response back to the original stream
        responseBody.Seek(0, SeekOrigin.Begin);
        await responseBody.CopyToAsync(originalBodyStream);
    }

    private static RateLimitHeaders? ExtractRateLimitHeaders(HttpContext context)
    {
        // Try to extract headers from the rate limiter lease
        // Note: The actual implementation depends on how you track the lease
        // This is a placeholder
        return null;
    }
}
``` 


### src > ImanSoftware.RateLimiter > Middlewares > UseRateLimiterExtensions 

```csharp 
﻿using ImanSoftware.RateLimiter.Core.Models;
using ImanSoftware.RateLimiter.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ImanSoftware.RateLimiter;

/// <summary>
/// Extension methods for adding the rate limiter middleware.
/// </summary>
public static class UseRateLimiterExtensions
{
    /// <summary>
    /// Adds the ImanSoftware Rate Limiter middleware to the pipeline.
    /// </summary>
    public static IApplicationBuilder UseImanRateLimiter(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        // Verify that the rate limiter services are registered
        using var scope = app.ApplicationServices.CreateScope();
        var options = scope.ServiceProvider.GetService<ImanRateLimiterOptions>();
        if (options is null)
        {
            throw new InvalidOperationException(
                "RateLimiterOptions is not registered. Call AddImanRateLimiter() in Program.cs.");
        }

        // Add the Microsoft rate limiter middleware first
        app.UseRateLimiter();

        // Add custom middleware for headers and logging
        app.UseMiddleware<RateLimiterMiddleware>();

        return app;
    }
}
``` 


### src > ImanSoftware.Serialization > ImanSoftware.Serialization 

```xml 
﻿<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" />
    <PackageReference Include="Newtonsoft.Json" />
  </ItemGroup>

  <PropertyGroup>
    <PackageId>ImanSoftware.Serialization</PackageId>
    <Version>1.0.2</Version>
    <Description>Unified JSON serialization abstraction with providers for System.Text.Json and Newtonsoft.Json.</Description>
    <PackageTags>json;serialization;newtonsoft;systemtextjson;abstraction</PackageTags>
  </PropertyGroup>
  
</Project>

``` 


### src > ImanSoftware.Serialization > Abstractions > IImanJsonSerializer 

```csharp 
﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ImanSoftware.Serialization;

/// <summary>
/// Contract for JSON serialization and deserialization.
/// Provides a unified abstraction over System.Text.Json or other libraries.
/// </summary>
public interface IImanJsonSerializer
{
    /// <summary>
    /// Serializes an object to a JSON string.
    /// </summary>
    string Serialize<T>(T value);

    /// <summary>
    /// Serializes an object to a JSON string with custom options.
    /// </summary>
    string Serialize<T>(T value, object? options = null);

    /// <summary>
    /// Deserializes a JSON string to an object of type T.
    /// </summary>
    T? Deserialize<T>(string json);

    /// <summary>
    /// Deserializes a JSON string to an object of type T with custom options.
    /// </summary>
    T? Deserialize<T>(string json, object? options = null);
}
``` 


### src > ImanSoftware.Serialization > Abstractions > JsonSerializerRegistration 

```csharp 
﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ImanSoftware.Serialization;

public static class ImanJsonSerializerRegistration
{
    public static IServiceCollection AddImanSerialization(
        this IServiceCollection services,
        SerializationDefaultImplementationType ImplementationType,
        Type? customImplementation = null)
    {
        if (customImplementation is not null)
        {
            services.AddSingleton(typeof(IImanJsonSerializer), customImplementation);
        }
        else
        {
            switch (ImplementationType)
            {
                case SerializationDefaultImplementationType.Newtonsoft:
                    services.TryAddSingleton<IImanJsonSerializer, NewtonsoftJsonSerializer>();
                    break;

                case SerializationDefaultImplementationType.SystemText:
                    services.TryAddSingleton<IImanJsonSerializer, SystemTextJsonSerializer>();
                    break;

                default:
                    services.TryAddSingleton<IImanJsonSerializer, NewtonsoftJsonSerializer>();
                    break;
            }
        }

        return services;
    }
}
``` 


### src > ImanSoftware.Serialization > Abstractions > SerializationDefaultImpelementationType 

```csharp 
﻿
namespace ImanSoftware.Serialization;

public enum SerializationDefaultImplementationType
{
    SystemText,
    Newtonsoft,
}


``` 


### src > ImanSoftware.Serialization > Infrastructure > JsonSerializerFactory 

```csharp 
﻿// ImanSoftware.Serialization/JsonSerializerFactory.cs
using System.Text.Json;
using Newtonsoft.Json;

namespace ImanSoftware.Serialization;

/// <summary>
/// Factory for creating IJsonSerializer instances with custom options.
/// </summary>
public static class JsonSerializerFactory
{
    /// <summary>
    /// Creates a System.Text.Json serializer with custom options.
    /// </summary>
    public static IImanJsonSerializer CreateSystemTextJsonSerializer(JsonSerializerOptions? options = null)
    {
        return new SystemTextJsonSerializer(options);
    }

    /// <summary>
    /// Creates a Newtonsoft.Json serializer with custom settings.
    /// </summary>
    public static IImanJsonSerializer CreateNewtonsoftJsonSerializer(JsonSerializerSettings? settings = null)
    {
        return new NewtonsoftJsonSerializer(settings);
    }

    /// <summary>
    /// Creates a System.Text.Json serializer with default options (CamelCase, not indented).
    /// </summary>
    public static IImanJsonSerializer CreateDefaultSystemTextJsonSerializer()
    {
        return new SystemTextJsonSerializer();
    }

    /// <summary>
    /// Creates a Newtonsoft.Json serializer with default settings (CamelCase, ignore nulls).
    /// </summary>
    public static IImanJsonSerializer CreateDefaultNewtonsoftJsonSerializer()
    {
        return new NewtonsoftJsonSerializer();
    }
}
``` 


### src > ImanSoftware.Serialization > Infrastructure > NewtonsoftJsonSerializer 

```csharp 
﻿using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ImanSoftware.Serialization;

/// <summary>
/// Implementation of IJsonSerializer using Newtonsoft.Json.
/// </summary>
public class NewtonsoftJsonSerializer : IImanJsonSerializer
{
    private readonly JsonSerializerSettings _defaultSettings;

    public NewtonsoftJsonSerializer(JsonSerializerSettings? settings = null)
    {
        _defaultSettings = settings ?? new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.None
        };
    }

    public string Serialize<T>(T value)
        => Serialize(value, null);

    public string Serialize<T>(T value, object? options = null)
    {
        var settings = options as JsonSerializerSettings ?? _defaultSettings;
        return JsonConvert.SerializeObject(value, settings);
    }

    public T? Deserialize<T>(string json)
        => Deserialize<T>(json, null);

    public T? Deserialize<T>(string json, object? options = null)
    {
        var settings = options as JsonSerializerSettings ?? _defaultSettings;
        return JsonConvert.DeserializeObject<T>(json, settings);
    }
}
``` 


### src > ImanSoftware.Serialization > Infrastructure > SystemTextJsonSerializer 

```csharp 
﻿using System.Text.Json;

namespace ImanSoftware.Serialization;


/// <summary>
/// Default implementation of IJsonSerializer using System.Text.Json.
/// </summary>
internal class SystemTextJsonSerializer : IImanJsonSerializer
{
    private readonly JsonSerializerOptions _defaultOptions;

    public SystemTextJsonSerializer(JsonSerializerOptions? options = null)
    {
        _defaultOptions = options ?? new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    public string Serialize<T>(T value)
        => Serialize(value, null);

    public string Serialize<T>(T value, object? options = null)
    {
        var serializerOptions = options as JsonSerializerOptions ?? _defaultOptions;
        return JsonSerializer.Serialize(value, serializerOptions);
    }

    public T? Deserialize<T>(string json)
        => Deserialize<T>(json, null);

    public T? Deserialize<T>(string json, object? options = null)
    {
        var serializerOptions = options as JsonSerializerOptions ?? _defaultOptions;
        return JsonSerializer.Deserialize<T>(json, serializerOptions);
    }
}
``` 


