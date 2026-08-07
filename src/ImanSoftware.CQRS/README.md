# 🧩 ImanSoftware.CQRS

**Version:** 1.0.0 | **Last Updated:** August 2026

> A lightweight, opinionated CQRS abstraction layer built on top of MediatR for modern .NET applications.

`ImanSoftware.CQRS` provides clean abstractions for implementing the Command Query Responsibility Segregation (CQRS) pattern. It introduces `ICommand`, `IQuery`, `ICommandHandler`, and `IQueryHandler` interfaces while leveraging MediatR under the hood.

---

## Installation

```bash
dotnet add package ImanSoftware.CQRS
```

## Why ImanSoftware.CQRS?

Instead of depending directly on MediatR throughout your application, this package provides semantic CQRS abstractions that improve readability and maintainability.

Benefits:

- Clean Architecture friendly
- Strongly-typed Commands and Queries
- Minimal boilerplate
- Built on MediatR
- Dependency Injection support
- Pipeline Behavior ready

---

# Features

- ✅ ICommand / ICommand<TResult>
- ✅ IQuery<TResult>
- ✅ ICommandHandler
- ✅ IQueryHandler
- ✅ AddCqrs() registration
- ✅ MediatR integration
- ✅ Pipeline behaviors
- ✅ Lightweight

---

# Register

```csharp
using ImanSoftware.CQRS.DependencyInjection;
using System.Reflection;

builder.Services.AddCqrs(
    assemblies: new[]
    {
        Assembly.GetExecutingAssembly()
    });
```

---

# Commands

```csharp
public sealed record CreateProductCommand(
    string Name,
    decimal Price) : ICommand<Guid>;
```

```csharp
public sealed class CreateProductCommandHandler
    : ICommandHandler<CreateProductCommand, Guid>
{
    public async Task<Guid> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        return Guid.NewGuid();
    }
}
```

---

# Queries

```csharp
public sealed record GetProductQuery(Guid Id)
    : IQuery<ProductDto>;
```

```csharp
public sealed class GetProductQueryHandler
    : IQueryHandler<GetProductQuery, ProductDto>
{
    public async Task<ProductDto> Handle(
        GetProductQuery request,
        CancellationToken cancellationToken)
    {
        return new ProductDto();
    }
}
```

---

# Sending Requests

```csharp
Guid id = await mediator.Send(new CreateProductCommand("Keyboard", 150));

ProductDto product = await mediator.Send(new GetProductQuery(id));
```

---

# Pipeline Behaviors

```csharp
builder.Services.AddCqrs(
    assemblies: new[] { Assembly.GetExecutingAssembly() },
    configure: config =>
    {
        config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        config.AddOpenBehavior(typeof(LoggingBehavior<,>));
    });
```

---

# Best Practices

- One command or query per file.
- Keep handlers focused.
- Validate commands before execution.
- Return DTOs from queries.
- Use pipeline behaviors for cross-cutting concerns.

---

# Dependencies

- MediatR
- Microsoft.Extensions.DependencyInjection.Abstractions

---

# Compatibility

- .NET 10+
- ASP.NET Core
- Console Applications
- Worker Services
- Blazor

---

# License

MIT License.
