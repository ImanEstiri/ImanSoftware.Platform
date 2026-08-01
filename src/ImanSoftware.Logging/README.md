# ImanSoftware.Logging

**Version:** 1.0.0 | **Last Updated:** August 2026

> A lightweight logging abstraction for .NET built on top of `Microsoft.Extensions.Logging`, providing a clean, testable API with full support for scopes and log levels.

`ImanSoftware.Logging` wraps the standard .NET logging infrastructure behind a simple adapter interface, making application code easier to test while remaining fully compatible with the Microsoft logging ecosystem.

---

## Installation

Install the package using the .NET CLI:

```bash
dotnet add package ImanSoftware.Logging
```

---

## Why LoggerAdapter?

Depending directly on `ILogger<T>` throughout your application can tightly couple business logic to the logging framework.

Instead of:

```csharp
private readonly ILogger<ProductService> _logger;
```

Use:

```csharp
private readonly ILoggerAdapter<ProductService> _logger;
```

This abstraction improves testability while preserving all capabilities of the built-in logging system.

Benefits include:

- Cleaner architecture
- Easier unit testing
- Consistent logging API
- Full support for logging scopes
- Compatible with Microsoft.Extensions.Logging

---

# Features

- ✅ `ILoggerAdapter<TCategoryName>`
- ✅ All standard log levels
- ✅ Generic `Log()` method
- ✅ Structured logging support
- ✅ Logging scopes (`BeginScope`)
- ✅ Open Generic Dependency Injection registration
- ✅ Built on `ILoggerFactory`
- ✅ Lightweight and dependency-free

---

# ILoggerAdapter<TCategoryName>

The primary abstraction for application logging.

Available methods:

- `LogInformation()`
- `LogWarning()`
- `LogError()`
- `LogDebug()`
- `LogTrace()`
- `LogCritical()`
- `Log()`
- `BeginScope()`

Example:

```csharp
_logger.LogInformation("Product created.");

_logger.LogWarning("Low inventory.");

_logger.LogError(exception, "Unexpected error.");
```

---

# LoggerAdapter<TCategoryName>

Default implementation built on top of `ILoggerFactory`.

Benefits include:

- Fully compatible with the Microsoft logging infrastructure
- Works with any configured logging provider
- Supports structured logging
- Supports scopes
- Easy to replace during testing

---

# Logging Scopes

Scopes allow related log entries to share contextual information.

Example:

```csharp
using (_logger.BeginScope(new
{
    ProductId = id
}))
{
    _logger.LogInformation("Fetching product...");

    _logger.LogInformation("Product loaded.");
}
```

Every log entry inside the scope automatically includes the scope properties, making log analysis and tracing much easier.

---

# Dependency Injection

Register the adapter in your application:

```csharp
services.AddLoggerAdapter();
```

The adapter is registered as an **Open Generic**, allowing it to be injected into any service:

```csharp
ILoggerAdapter<ProductService>
```

> **Note**
>
> If you are using **AddImanFramework()**, the logger adapter is registered automatically and no additional configuration is required.

---

# Example

```csharp
using ImanSoftware.Logging;

public class ProductService
{
    private readonly ILoggerAdapter<ProductService> _logger;

    public ProductService(
        ILoggerAdapter<ProductService> logger)
    {
        _logger = logger;
    }

    public async Task<Product> GetProductAsync(int id)
    {
        using (_logger.BeginScope(new
        {
            ProductId = id
        }))
        {
            _logger.LogInformation("Fetching product...");

            // Business logic...
        }
    }
}
```

---

# Unit Testing

Because logging is abstracted behind an interface, it can easily be mocked.

```csharp
var mock = new Mock<ILoggerAdapter<ProductService>>();

mock.Setup(x =>
    x.LogInformation(It.IsAny<string>()));
```

This keeps unit tests simple and independent of the logging infrastructure.

---

# Design Goals

- Clean abstraction
- Dependency Injection friendly
- High performance
- Structured logging support
- Scope support
- Easy to mock
- Framework independent
- Clean Architecture compatible
- Minimal API

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