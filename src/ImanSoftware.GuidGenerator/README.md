# ImanSoftware.GuidGenerator

**Version:** 1.0.0 | **Last Updated:** August 2026

> A flexible and testable GUID generation library for .NET with support for both **Sequential** and **Random** GUIDs.

`ImanSoftware.GuidGenerator` abstracts GUID generation behind a simple interface, allowing applications to use database-friendly sequential GUIDs or traditional random GUIDs through dependency injection.

---

## Installation

Install the package using the .NET CLI:

```bash
dotnet add package ImanSoftware.GuidGenerator
```

---

## Why GuidGenerator?

Calling `Guid.NewGuid()` directly tightly couples your code to a specific implementation and always produces random GUIDs.

In many database systems, sequential GUIDs significantly reduce index fragmentation and improve insert performance.

Instead of:

```csharp
var id = Guid.NewGuid();
```

Use:

```csharp
var id = _guidGenerator.NewGuid();
```

This approach improves testability while allowing the GUID generation strategy to be configured in one place.

Benefits include:

- Better database performance with sequential GUIDs
- Dependency Injection friendly
- Easy unit testing
- Configurable generation strategy
- Lightweight and dependency-free

---

# Features

- ✅ `IGuidGenerator` abstraction
- ✅ Sequential GUID generator
- ✅ Random GUID generator
- ✅ Configurable implementation
- ✅ Dependency Injection support
- ✅ Mock-friendly design
- ✅ Database-optimized sequential GUIDs
- ✅ Zero configuration

---

# IGuidGenerator

The core abstraction exposes a single method for generating GUIDs.

```csharp
public interface IGuidGenerator
{
    Guid NewGuid();
}
```

---

# SequentialGuidGenerator

Generates sequential GUIDs optimized for database indexes.

Benefits include:

- Reduced index fragmentation
- Faster inserts
- Better clustered index performance
- Suitable for SQL Server and similar databases

Example:

```csharp
Guid id = _guidGenerator.NewGuid();
```

---

# RandomGuidGenerator

Generates standard random GUIDs equivalent to `Guid.NewGuid()`.

Suitable for:

- Distributed systems
- Public identifiers
- General-purpose applications

---

# Dependency Injection

By default, the package registers the sequential implementation.

```csharp
services.AddGuidGenerator();
```

This registers:

```text
SequentialGuidGenerator
```

To use random GUIDs instead:

```csharp
services.AddGuidGenerator(GuidGeneratorImplementationType.Random);
```

---

# Example

```csharp
using ImanSoftware.GuidGenerator;

public class ProductService
{
    private readonly IGuidGenerator _guidGenerator;

    public ProductService(IGuidGenerator guidGenerator)
    {
        _guidGenerator = guidGenerator;
    }

    public void CreateProduct()
    {
        var id = _guidGenerator.NewGuid();

        // Business logic...
    }
}
```

---

# Unit Testing

Because GUID generation is abstracted behind an interface, it can easily be mocked.

```csharp
var mock = new Mock<IGuidGenerator>();

mock.Setup(x => x.NewGuid())
    .Returns(Guid.Parse("11111111-1111-1111-1111-111111111111"));
```

This makes tests deterministic and repeatable.

---

# Design Goals

- High performance
- Database-friendly
- Dependency Injection compatible
- Lightweight
- Easy to configure
- Mock-friendly
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