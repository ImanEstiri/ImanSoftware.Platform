# ImanSoftware.Framework

**Version:** 1.0.0 | **Last Updated:** August 2026

> The complete **ImanSoftware** development framework for .NET. Install one package, register one service, and get access to the entire ecosystem.

`ImanSoftware.Framework` is an umbrella package that bundles all ImanSoftware libraries into a single package. It simplifies application setup by automatically registering supported services and managing dependencies, allowing you to bootstrap a new project with minimal configuration.

---

## Installation

Install the package using the .NET CLI:

```bash
dotnet add package ImanSoftware.Framework
```

---

## Why ImanSoftware.Framework?

Modern applications rely on many foundational services such as caching, logging, serialization, JWT authentication, data access, and utility libraries.

Instead of installing and configuring each package individually, `ImanSoftware.Framework` provides a unified setup experience.

Benefits include:

- One package
- One registration method
- Automatic dependency management
- Clean Architecture friendly
- Consistent configuration
- Easy customization

---

# Included Packages

`ImanSoftware.Framework` includes the following libraries as dependencies:

- **ImanSoftware.Outcome**
- **ImanSoftware.Extensions**
- **ImanSoftware.Domain**
- **ImanSoftware.DateTimeProvider**
- **ImanSoftware.GuidGenerator**
- **ImanSoftware.Serialization**
- **ImanSoftware.Logging**
- **ImanSoftware.Caching**
- **ImanSoftware.CurrentUserAccessor**
- **ImanSoftware.Jwt**
- **ImanSoftware.DataAccess**
- **ImanSoftware.DataAccess.Dapper**
- **ImanSoftware.FileStorage**

All required package dependencies are resolved automatically.

---

# Features

- ✅ One-line application setup
- ✅ Automatic Dependency Injection registration
- ✅ Centralized configuration
- ✅ Configurable service registration
- ✅ Automatic dependency management
- ✅ Clean Architecture ready
- ✅ ASP.NET Core friendly
- ✅ Highly customizable

---

# AddImanFramework()

The `AddImanFramework()` extension method registers all selected framework services in the application's dependency injection container.

Example:

```csharp
builder.Services.AddImanFramework(
    new ImanFrameworkDependencyInjectionOptions
    {
        AddDateTimeProvider = true,
        AddGuidGenerator = true,
        AddJsonSerializer = true,
        AddCacheService = true,
        AddJwtService = true,
        JwtConfigureOptions = builder.Configuration
            .GetSection("Jwt")
            .Get<JwtOptions>(),
        AddDataAccess = true,
        ConnectionString = builder.Configuration
            .GetConnectionString("DefaultConnection")
    });
```

---

# ImanFrameworkDependencyInjectionOptions

Configure which framework components should be registered.

Example options include:

| Option | Description |
|---------|-------------|
| `AddDateTimeProvider` | Registers the DateTime provider |
| `AddGuidGenerator` | Registers the GUID generator |
| `AddJsonSerializer` | Registers the JSON serializer |
| `AddCacheService` | Registers the cache service |
| `AddJwtService` | Registers JWT services |
| `JwtConfigureOptions` | Configures JWT authentication |
| `AddDataAccess` | Registers data access services |
| `ConnectionString` | Database connection string |

Only the services you enable are registered.

---

# Custom Service Registration

`AddImanFramework()` also supports custom service registration, allowing additional services to be configured during framework initialization.

Example:

```csharp
builder.Services.AddImanFramework(
    options,
    services =>
    {
        services.AddScoped<IMyService, MyService>();

        services.AddSingleton<MyConfiguration>();
    });
```

This provides a centralized place for both framework and application service registration.

---

# Complete Example

```csharp
using ImanSoftware.Framework;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddImanFramework(
    new ImanFrameworkDependencyInjectionOptions
    {
        AddDateTimeProvider = true,
        AddGuidGenerator = true,
        AddJsonSerializer = true,
        AddCacheService = true,
        AddJwtService = true,
        JwtConfigureOptions = builder.Configuration
            .GetSection("Jwt")
            .Get<JwtOptions>(),
        AddDataAccess = true,
        ConnectionString = builder.Configuration
            .GetConnectionString("DefaultConnection")
    });

var app = builder.Build();

app.UseJwtAuthentication();

app.Run();
```

---

# Automatic Registrations

Depending on the selected options, the framework automatically registers services such as:

- Date & Time Provider
- GUID Generator
- JSON Serializer
- Logger Adapter
- Cache Service
- Current User Accessor
- JWT Services
- SQL Data Access
- Dapper Integration
- Local File Storage

This eliminates repetitive startup configuration across projects.

---

# Design Goals

- One-package experience
- Convention over configuration
- Dependency Injection first
- Modular architecture
- Lightweight
- Highly extensible
- Clean Architecture compatible
- Production ready
- Minimal setup

---

# Requirements

- .NET Standard 2.0+
- .NET 6+
- .NET 7+
- .NET 8+
- .NET 9+
- .NET 10.0+

---

# Included Dependencies

- **ImanSoftware.Outcome**
- **ImanSoftware.Extensions**
- **ImanSoftware.Domain**
- **ImanSoftware.DateTimeProvider**
- **ImanSoftware.GuidGenerator**
- **ImanSoftware.Serialization**
- **ImanSoftware.Logging**
- **ImanSoftware.Caching**
- **ImanSoftware.CurrentUserAccessor**
- **ImanSoftware.Jwt**
- **ImanSoftware.DataAccess**
- **ImanSoftware.DataAccess.Dapper**
- **ImanSoftware.FileStorage**

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