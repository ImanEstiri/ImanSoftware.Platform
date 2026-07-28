# 🚀 ImanSoftware.Framework

**A modular, production-ready .NET framework for building modern web applications with clean architecture, built-in best practices, and zero boilerplate.**

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=.net)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE.txt)
[![Author](https://img.shields.io/badge/Author-Iman%20Estiri-blue)](https://github.com/JavadEstiri)

---

## 📖 Table of Contents

- [Introduction](#-introduction)
- [Features](#-features)
- [Installation](#-installation)
- [Quick Start](#-quick-start)
- [Core Concepts](#-core-concepts)
  - [Result Pattern](#-result-pattern)
  - [Exception Handling](#️-exception-handling)
  - [Extensions](#️-extensions-30-methods)
- [Services & Configuration Options](#-services--configuration-options)
  - [DateTime Provider](#-datetime-provider)
  - [Guid Generator](#-guid-generator)
  - [Current User Accessor](#-current-user-accessor)
  - [JSON Serializer](#-json-serializer)
  - [Cache Service](#️-cache-service-in-memory)
  - [Logger Adapter](#-logger-adapter)
  - [JWT Service & Middleware](#-jwt-service--middleware)
  - [Data Access Layer](#️-data-access-layer-dapper)
  - [File Storage](#-file-storage-local-file-system)
- [Full Options Reference Table](#-full-options-reference-table)
- [Custom Dependency Injection](#-custom-dependency-injection)
- [Middleware](#-middleware)
- [Testing](#-testing)
- [Extending the Framework](#️-extending-the-framework)
- [Contributing](#-contributing)
- [License](#-license)
- [Author & Support](#-author--support)

---

## 🎯 Introduction

**ImanSoftware.Framework** is a lightweight yet powerful framework designed to accelerate development of ASP.NET Core applications. It provides a set of pre-built, battle-tested components that solve common problems in every project:

- ✅ Consistent error handling with the **Result Pattern**
- ✅ Built-in **Dependency Injection** with flexible, per-service configuration
- ✅ **Security** (JWT authentication, password hashing)
- ✅ **Data Access** with Dapper (Stored Procedures & Ad-hoc Queries)
- ✅ **Caching**, **Logging**, **Serialization**, and more

The framework follows **Clean Architecture** principles and is designed to be **extensible**, **testable**, and **developer-friendly**. Every service can be turned on/off and configured independently through a single strongly-typed options object.

---

## ✨ Features

| Category | Services |
|----------|----------|
| **Core** | Result Pattern, Custom Exceptions, Guard Clauses, Base Classes (`Entity`, `ValueObject`, `AggregateRoot`) |
| **DateTime** | `IDateTimeProvider` – abstracts `DateTime.Now` for testability |
| **Identity** | `IGuidGenerator` (Sequential & Random), `ICurrentUserAccessor<TUserId>` |
| **Serialization** | `IJsonSerializer` with `System.Text.Json` & `Newtonsoft.Json` support |
| **Caching** | `ICacheService` with In-Memory provider (Redis-ready) |
| **Logging** | `ILoggerAdapter<TCategoryName>` – structured logging abstraction |
| **Security** | `IJwtTokenService` + `JwtMiddleware` for authentication |
| **Data Access** | `ISqlDataAccessor` with Dapper (Stored Procedures, Queries, Transactions) |
| **File Storage** | `IFileStorage` – Upload, Download, Delete, Move, Copy, Metadata |
| **Utilities** | 30+ extension methods for `string`, `DateTime`, `Enum`, `IEnumerable`, `Exception` |

---

## 📦 Installation

### 1. Add the NuGet Package *(coming soon)*

```bash
dotnet add package ImanSoftware.Framework
```

### 2. Or Reference the Project Directly

```xml
<ProjectReference Include="..\ImanSoftware.Framework\ImanSoftware.Framework.csproj" />
```

### 3. Add Required NuGet Dependencies *(if using Central Package Management)*

```xml
<!-- Directory.Packages.props -->
<PackageVersion Include="Dapper" Version="2.1.35" />
<PackageVersion Include="Microsoft.Extensions.Caching.Memory" Version="8.0.0" />
<PackageVersion Include="Microsoft.Extensions.Logging.Abstractions" Version="8.0.0" />
<PackageVersion Include="Microsoft.AspNetCore.Http.Abstractions" Version="2.2.0" />
<PackageVersion Include="System.IdentityModel.Tokens.Jwt" Version="7.0.0" />
<PackageVersion Include="Microsoft.IdentityModel.Tokens" Version="7.0.0" />
<PackageVersion Include="Newtonsoft.Json" Version="13.0.3" />
<PackageVersion Include="Microsoft.Data.SqlClient" Version="5.0.0" />
```

---

## 🚀 Quick Start

### 1. Register the Framework in `Program.cs`

```csharp
using ImanSoftware.Framework.FrameworkDependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Register all framework services with default configurations
builder.Services.AddImanFramework();

var app = builder.Build();
app.Run();
```

### 2. Use Services in Your Controllers or Services

```csharp
public class ProductService
{
    private readonly IDateTimeProvider _dateTime;
    private readonly ILoggerAdapter<ProductService> _logger;
    private readonly ISqlDataAccessor _db;

    public ProductService(
        IDateTimeProvider dateTime,
        ILoggerAdapter<ProductService> logger,
        ISqlDataAccessor db)
    {
        _dateTime = dateTime;
        _logger = logger;
        _db = db;
    }

    public async Task<Result<Product>> GetProductAsync(int id)
    {
        _logger.LogInformation("Fetching product {ProductId} at {Time}", id, _dateTime.Now);
        return await _db.QuerySingleAsync<Product>("GetProductById", new { Id = id }, CommandType.StoredProcedure);
    }
}
```

---

## 🧩 Core Concepts

### 📦 Result Pattern

All operations return `Result` or `Result<T>` instead of throwing exceptions or returning `null`.

```csharp
public async Task<Result<Product>> GetProductAsync(int id)
{
    var product = await _db.QuerySingleAsync<Product>(...);
    if (product.Data is null)
        return Result<Product>.Failure(new AppError("Product not found", "NOT_FOUND", ErrorType.NotFound));

    return product;
}

// Usage
var result = await service.GetProductAsync(42);
if (result.IsSuccess)
{
    var product = result.Data; // Safe to access
}
else
{
    Console.WriteLine($"Error: {result.Error.Message} ({result.Error.Code})");
}
```

### 🛡️ Exception Handling

Base exceptions convert to `Result` automatically.

```csharp
// Throw exception
throw new NotFoundException($"Product {id} not found.");

// Catch and convert to Result
try { ... }
catch (FrameworkException ex)
{
    return ex.ToResult(); // Returns Result.Failure
}
```

### 🛠️ Extensions (30+ Methods)

```csharp
// String
"hello world".ToCamelCase();           // "helloWorld"
"HelloWorld".ToSnakeCase();            // "hello_world"
"  hello  ".NormalizeWhitespace();     // "hello"

// DateTime
DateTime.Now.StartOfDay();             // 2024-01-01 00:00:00
DateTime.Now.Age(birthDate);           // 25

// Enum
OrderStatus.Shipped.GetDescription();  // "Shipped"
OrderStatus.Pending.ToInt();           // 1

// IEnumerable
var chunks = items.Chunk(10);          // Split into chunks of 10
```

---

## 🧰 Services & Configuration Options

Every service in the framework is registered through the single `AddImanFramework(...)` extension method, configured via an instance of `ImanFrameworkDependencyInjectionOptions`. Each service exposes:

1. An **`Add<Service>` boolean flag** to enable/disable it.
2. Optional **configuration properties** (connection strings, JWT settings, implementation type, etc.).
3. An optional **custom registration delegate** to override the default implementation.

### 🕒 DateTime Provider

| Option | Type | Default | Description |
|---|---|---|---|
| `AddDateTimeProvider` | `bool` | `true` | Enables/disables the service |
| `DateTimeProviderCustomRegistration` | `Action<IServiceCollection>` | `null` | Overrides the default implementation |

```csharp
// Register (default: enabled)
builder.Services.AddImanFramework();

// Use
public class OrderService
{
    private readonly IDateTimeProvider _dateTime;
    public OrderService(IDateTimeProvider dateTime) => _dateTime = dateTime;

    public void CreateOrder()
    {
        var now = _dateTime.Now;   // Returns System.DateTime.Now
        var today = _dateTime.Today;
    }
}
```

### 🆔 Guid Generator

| Option | Type | Default | Description |
|---|---|---|---|
| `AddGuidGenerator` | `bool` | `true` | Enables/disables the service |
| `GuidGeneratorImplementationType` | `GuidGeneratorDefaultImplementationType` | `Sequential` | `Sequential` (DB-friendly) or `Random` |

```csharp
// Configuration
builder.Services.AddImanFramework(new ImanFrameworkDependencyInjectionOptions
{
    AddGuidGenerator = true,
    GuidGeneratorImplementationType = GuidGeneratorDefaultImplementationType.Sequential
});

// Use
public class ProductService
{
    private readonly IGuidGenerator _guidGenerator;
    public ProductService(IGuidGenerator guidGenerator) => _guidGenerator = guidGenerator;

    public void CreateProduct()
    {
        var id = _guidGenerator.NewGuid(); // Sequential GUID (DB-friendly)
    }
}
```

### 👤 Current User Accessor

| Option | Type | Default | Description |
|---|---|---|---|
| `AddCurrentUserAccessor` | `bool` | `true` | Enables/disables the service (registered as an open generic `ICurrentUserAccessor<TUserId>`) |

```csharp
// Register (open generic)
builder.Services.AddImanFramework();

// Use with Guid
public class ProfileService
{
    private readonly ICurrentUserAccessor<Guid> _userAccessor;
    public ProfileService(ICurrentUserAccessor<Guid> userAccessor) => _userAccessor = userAccessor;

    public void ShowProfile()
    {
        var userId = _userAccessor.GetCurrentUserId(); // Returns Guid
        var user = _userAccessor.GetCurrentUser();      // Returns ClaimsPrincipal
    }
}

// Use with int
public class LegacyService
{
    private readonly ICurrentUserAccessor<int> _userAccessor;
    // ...
}
```

### 📦 JSON Serializer

| Option | Type | Default | Description |
|---|---|---|---|
| `AddJsonSerializer` | `bool` | `true` | Enables/disables the service |
| `SerializationImplementationType` | `SerializationDefaultImplementationType` | `SystemTextJson` | `SystemTextJson` or `Newtonsoft` |

```csharp
// Configuration (choose System.Text.Json or Newtonsoft)
builder.Services.AddImanFramework(new ImanFrameworkDependencyInjectionOptions
{
    AddJsonSerializer = true,
    SerializationImplementationType = SerializationDefaultImplementationType.SystemTextJson
});

// Use
public class ProductService
{
    private readonly IJsonSerializer _json;
    public ProductService(IJsonSerializer json) => _json = json;

    public string Serialize(Product product) => _json.Serialize(product);
    public Product? Deserialize(string json) => _json.Deserialize<Product>(json);
}
```

### 🗄️ Cache Service (In-Memory)

| Option | Type | Default | Description |
|---|---|---|---|
| `AddCacheService` | `bool` | `false` | Enables/disables the service |
| `CacheServiceCustomRegistration` | `Action<IServiceCollection>` | `null` | Overrides the default in-memory implementation (e.g., to plug in Redis) |

```csharp
// Register
builder.Services.AddImanFramework(new ImanFrameworkDependencyInjectionOptions
{
    AddCacheService = true
});

// Use
public class ProductService
{
    private readonly ICacheService _cache;
    public ProductService(ICacheService cache) => _cache = cache;

    public async Task<Product> GetProductAsync(int id)
    {
        var key = $"product_{id}";
        return _cache.GetOrSet(key, () => GetFromDatabase(id), TimeSpan.FromMinutes(5));
    }
}
```

### 📝 Logger Adapter

| Option | Type | Default | Description |
|---|---|---|---|
| `AddLoggerAdapter` | `bool` | `true` | Enables/disables the service (registered as an open generic `ILoggerAdapter<TCategoryName>`) |

```csharp
// Register (default: enabled)
builder.Services.AddImanFramework();

// Use
public class ProductService
{
    private readonly ILoggerAdapter<ProductService> _logger;
    public ProductService(ILoggerAdapter<ProductService> logger) => _logger = logger;

    public async Task ProcessOrderAsync(Order order)
    {
        using (_logger.BeginScope(new { OrderId = order.Id }))
        {
            _logger.LogInformation("Starting order processing.");
            // All logs inside this scope include OrderId
        }
    }
}
```

### 🔐 JWT Service & Middleware

| Option | Type | Default | Description |
|---|---|---|---|
| `AddJwtService` | `bool` | `false` | Enables/disables the JWT token service |
| `JwtConfigureOptions` | `JwtOptions` | `null` | `SecretKey`, `Issuer`, `Audience`, `ExpiryMinutes` |
| `JwtCustomImplementation` | `Type` | `null` | Replaces the default `IJwtTokenService` implementation |

**`appsettings.json`:**

```json
{
  "Jwt": {
    "SecretKey": "your-256-bit-secret-key-here",
    "Issuer": "MyApp",
    "Audience": "MyApp.Users",
    "ExpiryMinutes": 120
  }
}
```

**Registration:**

```csharp
var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()!;

builder.Services.AddImanFramework(new ImanFrameworkDependencyInjectionOptions
{
    AddJwtService = true,
    JwtConfigureOptions = jwtOptions
});

// Enable the middleware (must be called after UseRouting())
app.UseJwtAuthentication(); // Extracts token & sets HttpContext.User
```

**Usage:**

```csharp
public class AuthService
{
    private readonly IJwtTokenService _jwt;
    public AuthService(IJwtTokenService jwt) => _jwt = jwt;

    public string Login(string username, string password)
    {
        // Validate user
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
        return _jwt.GenerateToken(claims);
    }
}
```

### 🗃️ Data Access Layer (Dapper)

| Option | Type | Default | Description |
|---|---|---|---|
| `AddDataAccess` | `bool` | `false` | Enables/disables the service |
| `ConnectionString` | `string` | `null` | SQL Server connection string |
| `DataAccessCustomRegistration` | `Action<IServiceCollection>` | `null` | Overrides the default `ISqlDataAccessor` implementation |

**`appsettings.json`:**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=MyDb;User Id=sa;Password=MyPass;"
  }
}
```

**Registration:**

```csharp
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddImanFramework(new ImanFrameworkDependencyInjectionOptions
{
    AddDataAccess = true,
    ConnectionString = connectionString
});
```

**Usage:**

```csharp
public class ProductService
{
    private readonly ISqlDataAccessor _db;
    public ProductService(ISqlDataAccessor db) => _db = db;

    // Stored Procedure
    public async Task<Result<Product>> GetProductAsync(int id)
    {
        return await _db.QuerySingleAsync<Product>(
            "GetProductById",
            new { Id = id },
            CommandType.StoredProcedure);
    }

    // Ad-hoc Query
    public async Task<Result<IEnumerable<Product>>> GetActiveProductsAsync()
    {
        return await _db.QueryAsync<Product>(
            "SELECT * FROM Products WHERE IsActive = 1",
            commandType: CommandType.Text);
    }

    // Transaction
    public async Task<Result> UpdateProductStockAsync(int productId, int newStock)
    {
        using var scope = await _db.BeginTransactionAsync();
        if (!scope.IsSuccess) return Result.Failure(scope.Error);

        try
        {
            var result = await _db.ExecuteAsync(
                "UpdateProductStock",
                new { ProductId = productId, NewStock = newStock },
                CommandType.StoredProcedure,
                scope.Data.Transaction);

            if (!result.IsSuccess)
            {
                await scope.Data.RollbackAsync();
                return Result.Failure(result.Error);
            }

            await scope.Data.CommitAsync();
            return Result.Successful();
        }
        catch
        {
            await scope.Data.RollbackAsync();
            throw;
        }
    }
}
```

### 📁 File Storage (Local File System)

| Option | Type | Default | Description |
|---|---|---|---|
| `AddFileStorage` | `bool` | `false` | Enables/disables the service *(coming soon)* |
| `FileStorageOptions.RootPath` | `string` | `null` | Root directory for stored files |

```csharp
// Register (coming soon)
builder.Services.AddImanFramework(new ImanFrameworkDependencyInjectionOptions
{
    AddFileStorage = true,
    FileStorageOptions = new FileStorageOptions
    {
        RootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads")
    }
});

// Use
public class FileService
{
    private readonly IFileStorage _fileStorage;
    public FileService(IFileStorage fileStorage) => _fileStorage = fileStorage;

    public async Task<Result<string>> UploadAsync(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        return await _fileStorage.UploadAsync(stream, file.FileName);
    }
}
```

---

## 📋 Full Options Reference Table

All options below belong to `ImanFrameworkDependencyInjectionOptions`, passed into `builder.Services.AddImanFramework(options)`.

| Option | Type | Default | Belongs to Service | Purpose |
|---|---|---|---|---|
| `AddDateTimeProvider` | `bool` | `true` | DateTime Provider | Enable/disable `IDateTimeProvider` |
| `DateTimeProviderCustomRegistration` | `Action<IServiceCollection>` | `null` | DateTime Provider | Custom implementation |
| `AddGuidGenerator` | `bool` | `true` | Guid Generator | Enable/disable `IGuidGenerator` |
| `GuidGeneratorImplementationType` | `GuidGeneratorDefaultImplementationType` | `Sequential` | Guid Generator | Choose `Sequential` or `Random` |
| `AddCurrentUserAccessor` | `bool` | `true` | Current User Accessor | Enable/disable `ICurrentUserAccessor<TUserId>` |
| `AddJsonSerializer` | `bool` | `true` | JSON Serializer | Enable/disable `IJsonSerializer` |
| `SerializationImplementationType` | `SerializationDefaultImplementationType` | `SystemTextJson` | JSON Serializer | Choose `SystemTextJson` or `Newtonsoft` |
| `AddCacheService` | `bool` | `false` | Cache Service | Enable/disable `ICacheService` |
| `CacheServiceCustomRegistration` | `Action<IServiceCollection>` | `null` | Cache Service | Plug in Redis or another provider |
| `AddLoggerAdapter` | `bool` | `true` | Logger Adapter | Enable/disable `ILoggerAdapter<TCategoryName>` |
| `AddJwtService` | `bool` | `false` | JWT Service | Enable/disable `IJwtTokenService` |
| `JwtConfigureOptions` | `JwtOptions` | `null` | JWT Service | `SecretKey`, `Issuer`, `Audience`, `ExpiryMinutes` |
| `JwtCustomImplementation` | `Type` | `null` | JWT Service | Replace default implementation (type-based) |
| `AddDataAccess` | `bool` | `false` | Data Access | Enable/disable `ISqlDataAccessor` |
| `ConnectionString` | `string` | `null` | Data Access | SQL Server connection string |
| `DataAccessCustomRegistration` | `Action<IServiceCollection>` | `null` | Data Access | Custom data-access implementation |
| `AddFileStorage` | `bool` | `false` | File Storage | Enable/disable `IFileStorage` *(coming soon)* |
| `FileStorageOptions` | `FileStorageOptions` | `null` | File Storage | `RootPath` and future storage settings |

> 💡 **Tip:** Calling `AddImanFramework()` with no arguments registers every service with its default configuration. Pass an `ImanFrameworkDependencyInjectionOptions` instance only when you need to enable optional services (Cache, JWT, Data Access, File Storage) or override an implementation.

---

## 🔧 Custom Dependency Injection

The framework supports two ways to override any default implementation:

### Option A — `Action<IServiceCollection>` (Recommended)

```csharp
builder.Services.AddImanFramework(new ImanFrameworkDependencyInjectionOptions
{
    AddDateTimeProvider = true,
    DateTimeProviderCustomRegistration = services =>
    {
        services.TryAddSingleton<IDateTimeProvider, CustomDateTimeProvider>();
    }
});
```

### Option B — `ImanFrameworkDependencyInjectionCustomImplementation<T>` (Type-Based)

```csharp
builder.Services.AddImanFramework(new ImanFrameworkDependencyInjectionOptions
{
    AddJwtService = true,
    JwtCustomImplementation = typeof(MyCustomJwtService) // Implicitly converts from Type
});
```

---

## ⚡ Middleware

### JWT Authentication Middleware

```csharp
app.UseJwtAuthentication(); // Must be called after UseRouting()
```

---

## 🧪 Testing

The framework is designed for testability — every service is exposed as an interface, so it can be mocked in isolation:

```csharp
// Mock IDateTimeProvider
var mockDateTime = new Mock<IDateTimeProvider>();
mockDateTime.Setup(x => x.Now).Returns(new DateTime(2024, 1, 1));

// Mock ISqlDataAccessor
var mockDb = new Mock<ISqlDataAccessor>();
mockDb.Setup(x => x.QuerySingleAsync<Product>(...))
      .ReturnsAsync(Result<Product>.Successful(new Product()));

var service = new ProductService(mockDateTime.Object, mockDb.Object);
```

---

## 🛠️ Extending the Framework

### 1. Add a New Service

1. Create the interface in `Abstractions/NewService/INewService.cs`
2. Create the implementation in `Infrastructure/NewService/NewService.cs`
3. Create the internal registration in `Abstractions/NewService/NewServiceRegistration.cs`
4. Add the corresponding option in `ImanFrameworkDependencyInjectionOptions`
5. Register it inside `FrameworkDependencyInjection.AddImanFramework`

### 2. Provide a Custom Implementation

```csharp
public class MyCustomDataAccessor : ISqlDataAccessor
{
    // Implement all methods
}

// Register
builder.Services.AddImanFramework(new ImanFrameworkDependencyInjectionOptions
{
    AddDataAccess = true,
    DataAccessCustomRegistration = services =>
    {
        services.TryAddScoped<ISqlDataAccessor, MyCustomDataAccessor>();
    }
});
```

---

## 🧑‍💻 Contributing

We welcome contributions! Please:

1. Fork the repository
2. Create a feature branch
3. Write tests
4. Submit a pull request

---

## 📄 License

This project is licensed under the **MIT License** — see the [LICENSE.txt](LICENSE.txt) file for details.

---

## 🙏 Acknowledgments

- Built with ❤️ by **ImanSoftware**
- Powered by **.NET 10**
- Inspired by **Clean Architecture** and **Domain-Driven Design**

---

## 👤 Author & Support

**Iman Estiri**
GitHub: [github.com/JavadEstiri](https://github.com/JavadEstiri?tab=repositories)

- 🐛 Issues & Feature Requests: open an issue on the repository above
- 📖 Documentation: this README (kept up to date alongside the codebase)

Made with ❤️ in Iran 🇮🇷
