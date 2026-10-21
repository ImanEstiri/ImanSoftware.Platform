# ImanSoftware.Platform

[![NuGet](https://img.shields.io/nuget/v/ImanSoftware.Framework?logo=nuget&label=Framework)](https://www.nuget.org/packages/ImanSoftware.Framework)
[![Downloads](https://img.shields.io/nuget/dt/ImanSoftware.Framework?logo=nuget&label=downloads)](https://www.nuget.org/profiles/ImanSoftware)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE.txt)

**Modular .NET 10 building blocks, published as independent NuGet packages.**
Outcome/Result pattern, CQRS, Dapper data access, JWT, rate limiting, caching, file storage and more.
Take a single package, or install the umbrella `ImanSoftware.Framework` and switch services on as you need them.

---

## Why this exists

Most ASP.NET Core projects start with the same plumbing: error handling without exceptions, a data-access layer, JWT, caching, logging, a clock you can mock in tests. This repository packages that plumbing as small libraries that:

- **work alone**: each package has its own NuGet package and few dependencies;
- **work together**: they share one error model (`Outcome`), so a failure from the data layer flows up through your services unchanged;
- **stay replaceable**: every service is an interface, and every default can be swapped for your own implementation.

## Packages

All packages are on NuGet: **[nuget.org/profiles/ImanSoftware](https://www.nuget.org/profiles/ImanSoftware)**

| Package | What it gives you |
|---|---|
| [`ImanSoftware.Outcome`](https://www.nuget.org/packages/ImanSoftware.Outcome) | `Outcome` / `Outcome<T>` result pattern, typed errors, exception types that convert to outcomes |
| [`ImanSoftware.CQRS`](https://www.nuget.org/packages/ImanSoftware.CQRS) | `ICommand`, `IQuery` and their handlers on top of MediatR |
| [`ImanSoftware.DataAccess`](https://www.nuget.org/packages/ImanSoftware.DataAccess) | Database-agnostic abstractions: `ISqlDataAccessor`, connection factory, transaction scope |
| [`ImanSoftware.DataAccess.Dapper`](https://www.nuget.org/packages/ImanSoftware.DataAccess.Dapper) | Dapper implementation for stored procedures, ad-hoc queries and transactions |
| [`ImanSoftware.Jwt`](https://www.nuget.org/packages/ImanSoftware.Jwt) | Token generation and validation, plus authentication middleware |
| [`ImanSoftware.CurrentUserAccessor`](https://www.nuget.org/packages/ImanSoftware.CurrentUserAccessor) | Current user from `HttpContext`, generic over `TUserId` (Guid, int, string) |
| [`ImanSoftware.RateLimiter`](https://www.nuget.org/packages/ImanSoftware.RateLimiter) | Named policies, fixed-window and other strategies, middleware and attribute, metrics |
| [`ImanSoftware.Caching`](https://www.nuget.org/packages/ImanSoftware.Caching) | `ICacheService` with an in-memory provider and a `GetOrSet` pattern |
| [`ImanSoftware.FileStorage`](https://www.nuget.org/packages/ImanSoftware.FileStorage) | Read, write, move, delete, enumerate, search, plus optional upload/download |
| [`ImanSoftware.Serialization`](https://www.nuget.org/packages/ImanSoftware.Serialization) | One JSON abstraction with `System.Text.Json` and Newtonsoft providers |
| [`ImanSoftware.Logging`](https://www.nuget.org/packages/ImanSoftware.Logging) | `ILoggerAdapter<T>`: structured logging with scopes over `ILogger` |
| [`ImanSoftware.DateTimeProvider`](https://www.nuget.org/packages/ImanSoftware.DateTimeProvider) | Abstraction over `DateTime.Now` for testable code |
| [`ImanSoftware.GuidGenerator`](https://www.nuget.org/packages/ImanSoftware.GuidGenerator) | Sequential (database-friendly) and random GUIDs |
| [`ImanSoftware.Domain`](https://www.nuget.org/packages/ImanSoftware.Domain) | DDD building blocks: entities, value objects (`Money`, `EmailAddress`, `PhoneNumber`, `Address`, `DateRange` and more) |
| [`ImanSoftware.Extensions`](https://www.nuget.org/packages/ImanSoftware.Extensions) | Extension methods for string, DateTime, Enum, IEnumerable, Exception, object |
| [`ImanSoftware.Branding`](https://www.nuget.org/packages/ImanSoftware.Branding) | Blazor badge component with contact information |
| [`ImanSoftware.Framework`](https://www.nuget.org/packages/ImanSoftware.Framework) | **Umbrella package**: bundles the modules above and registers them from one place |

> Looking for the dependency analyzer? **DepLens** lives in its own repository and is built on these packages: [ImanEstiri/DepLens](https://github.com/ImanEstiri/DepLens).

## Installation

Use one package:

```bash
dotnet add package ImanSoftware.Outcome
```

Or use everything through the umbrella package:

```bash
dotnet add package ImanSoftware.Framework
```

Requires **.NET 10**.

## Quick start

### 1. Return outcomes instead of throwing

```csharp
using ImanSoftware.Outcomes;

public Outcome<Product> GetProduct(int id)
{
    var product = _repository.Find(id);

    if (product is null)
        return Outcome.Failure<Product>(
            OutcomeError.Create("Product not found", "PRODUCT_NOT_FOUND", OutcomeErrorType.NotFound));

    return Outcome.Successful(product);
}

var result = GetProduct(42);

if (result.IsSuccess)
    Console.WriteLine(result.Data.Name);   // Data throws on a failed outcome, so check IsSuccess first
else
    Console.WriteLine($"{result.Error.Code}: {result.Error.Message}");
```

`Outcome<T>` also converts implicitly from `T`, so `return product;` works for the success path.

### 2. Register services from one place

```csharp
using ImanSoftware.Framework.FrameworkDependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddImanFramework(new ImanFrameworkDependencyInjectionOptions
{
    // On by default: DateTimeProvider, GuidGenerator, CurrentUserAccessor, JsonSerializer
    // Off by default: enable what you need
    AddCacheService = true,

    AddDataAccess = true,
    DataAccessConnectionString = builder.Configuration.GetConnectionString("Default"),
    AddDapperDataAccess = true,
    DapperConnectionString = builder.Configuration.GetConnectionString("Default"),

    AddJwtService = true,
    JwtConfigureOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()
});

var app = builder.Build();
app.UseJwtAuthentication();   // call after UseRouting()
app.Run();
```

Each option has a matching `...CustomImplementation` or `...CustomRegistration` hook, so you can replace any default.

### 3. Data access that returns outcomes

```csharp
public class ProductService(ISqlDataAccessor db)
{
    public Task<Outcome<Product>> GetAsync(int id) =>
        db.QuerySingleAsync<Product>(
            "GetProductById",
            new { Id = id },
            CommandType.StoredProcedure);

    public Task<Outcome<IEnumerable<Product>>> GetActiveAsync() =>
        db.QueryAsync<Product>("SELECT * FROM Products WHERE IsActive = 1");
}
```

`ISqlDataAccessor` supports stored procedures, ad-hoc queries, scalar and non-query commands, an optional `IDbTransaction` and `CancellationToken` on every call.

### 4. Rate limiting with named policies

```csharp
builder.Services.AddImanRateLimiter(options =>
{
    options.DefaultPolicy = new RateLimitPolicy { PermitLimit = 100, Window = TimeSpan.FromMinutes(1) };
    options.Policies["login"] = new RateLimitPolicy { PermitLimit = 5, Window = TimeSpan.FromMinutes(1) };
});

app.UseImanRateLimiter();
```

```csharp
[EnableImanRateLimiting(PolicyName = "login")]
[HttpPost("login")]
public IActionResult Login(LoginRequest request) { /* ... */ }
```

### 5. CQRS

```csharp
builder.Services.AddCqrs([typeof(Program).Assembly]);
```

Implement `ICommand`, `IQuery`, `ICommandHandler` and `IQueryHandler` from `ImanSoftware.CQRS`.

## Design principles

- **One error model.** Every operation that can fail returns `Outcome` or `Outcome<T>`; exceptions are for the unexpected.
- **Interfaces first.** Every service is an abstraction with a default implementation you can override.
- **Opt-in services.** Nothing heavy is registered unless you ask for it.
- **Testable by default.** Time, GUIDs, current user and logging are all behind interfaces, so they can be mocked.

## Repository layout

```
src/
  ImanSoftware.Outcome/          ImanSoftware.CQRS/
  ImanSoftware.DataAccess/       ImanSoftware.DataAccess.Dapper/
  ImanSoftware.Jwt/              ImanSoftware.CurrentUserAccessor/
  ImanSoftware.RateLimiter/      ImanSoftware.Caching/
  ImanSoftware.FileStorage/      ImanSoftware.Serialization/
  ImanSoftware.Logging/          ImanSoftware.DateTimeProvider/
  ImanSoftware.GuidGenerator/    ImanSoftware.Domain/
  ImanSoftware.Extensions/       ImanSoftware.Branding/
  ImanSoftware.Framework/        (umbrella)
```

Packages are built with central package management (`Directory.Packages.props`) and published with `publish.ps1`.

## Contributing

Issues and pull requests are welcome. Please open an issue first for larger changes, and keep each package focused on one concern.

## Security

See [SECURITY.md](SECURITY.md) for how to report a vulnerability.

## License

MIT. See [LICENSE.txt](LICENSE.txt).

## Author

**Iman Estiri**

- GitHub: [github.com/ImanEstiri](https://github.com/ImanEstiri)
- NuGet: [nuget.org/profiles/ImanSoftware](https://www.nuget.org/profiles/ImanSoftware)
