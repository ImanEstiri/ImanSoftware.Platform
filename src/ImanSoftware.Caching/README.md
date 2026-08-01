# ImanSoftware.Caching

**Version:** 1.0.0 | **Last Updated:** August 2026

> A lightweight and extensible caching abstraction for .NET with a default in-memory implementation.

`ImanSoftware.Caching` provides a clean caching API that decouples your application from any specific cache provider. The default implementation uses `IMemoryCache` and integrates with `IJsonSerializer` for flexible object serialization.

---

## Installation

Install the package using the .NET CLI:

```bash
dotnet add package ImanSoftware.Caching
```

---

## Why CacheService?

Caching is a common optimization, but coupling your application directly to a cache implementation makes future changes more difficult.

Instead of:

```csharp
_memoryCache.Set(key, value);
```

Use:

```csharp
_cache.Set(key, value);
```

This abstraction allows the underlying cache provider to change without affecting your business logic.

Benefits include:

- Clean architecture
- Improved application performance
- Dependency Injection friendly
- Easy unit testing
- Replaceable cache implementations

---

# Features

- ✅ `ICacheService` abstraction
- ✅ In-memory cache implementation
- ✅ Object serialization support
- ✅ Configurable expiration
- ✅ Thread-safe `GetOrSet()`
- ✅ Race condition protection
- ✅ Dependency Injection support
- ✅ Lightweight and extensible

---

# ICacheService

The core caching abstraction.

Available methods:

- `Get<T>()`
- `Set()`
- `Remove()`
- `Exists()`
- `GetOrSet()`

Example:

```csharp
_cache.Set("product", product);

var item = _cache.Get<Product>("product");

bool exists = _cache.Exists("product");

_cache.Remove("product");
```

---

# MemoryCacheService

The default implementation is built on top of `IMemoryCache`.

Features include:

- High-performance in-memory caching
- Automatic object serialization using `IJsonSerializer`
- Configurable expiration
- Thread-safe operations
- Suitable for ASP.NET Core applications

---

# Cache Expiration

Specify an expiration time when storing values.

```csharp
_cache.Set(
    "products",
    products,
    TimeSpan.FromMinutes(10));
```

Expired items are automatically removed by the underlying cache implementation.

---

# GetOrSet()

The `GetOrSet()` method simplifies cache-aside scenarios.

If the value exists, it is returned immediately.

Otherwise:

1. The factory function is executed.
2. The result is cached.
3. The cached value is returned.

```csharp
var product = _cache.GetOrSet(
    key,
    () => GetFromDatabase(id),
    TimeSpan.FromMinutes(5));
```

---

# Thread Safety

`GetOrSet()` uses synchronization internally to prevent multiple concurrent requests from executing the same expensive operation.

This helps prevent:

- Duplicate database queries
- Race conditions
- Cache stampedes

---

# Dependency Injection

Register the cache service in your application:

```csharp
services.AddCacheService();
```

After registration, `ICacheService` can be injected into any service.

---

# Example

```csharp
using ImanSoftware.Caching;

public class ProductService
{
    private readonly ICacheService _cache;

    public ProductService(ICacheService cache)
    {
        _cache = cache;
    }

    public async Task<Product> GetProductAsync(int id)
    {
        var key = $"product_{id}";

        return _cache.GetOrSet(
            key,
            () => GetFromDatabase(id),
            TimeSpan.FromMinutes(5));
    }
}
```

---

# Unit Testing

Because caching is abstracted behind an interface, it can easily be mocked.

```csharp
var mock = new Mock<ICacheService>();

mock.Setup(x =>
    x.Get<Product>("product_1"))
    .Returns(product);
```

This keeps your tests isolated from the underlying cache implementation.

---

# Design Goals

- High performance
- Clean abstraction
- Dependency Injection friendly
- Thread-safe operations
- Cache provider independent
- Lightweight
- Extensible
- Mock-friendly
- Clean Architecture compatible

---

# Requirements

- .NET Standard 2.0+
- .NET 6+
- .NET 7+
- .NET 8+
- .NET 9+
- .NET 10.0+

---

# Author

**Iman Estiri**

📧 contact.imanestiri@gmail.com

📧 dev.imanestiri@gmail.com

GitHub:

https://github.com/ImanEstiri

---

# License

This project is licensed under the MIT License.