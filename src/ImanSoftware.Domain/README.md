# ImanSoftware.Domain

**Version:** 1.0.0 | **Last Updated:** August 2026

> A lightweight foundation for building Domain-Driven Design (DDD) applications in .NET.

`ImanSoftware.Domain` provides the essential building blocks required to implement a clean and maintainable domain layer. It includes base entities, value objects, aggregate roots, specifications, repositories, domain events, and common domain models while remaining infrastructure-independent.

---

## Installation

Install the package using the .NET CLI:

```bash
dotnet add package ImanSoftware.Domain
```

---

## Why ImanSoftware.Domain?

Building a rich domain model often requires the same foundational components across projects.

This package provides reusable abstractions so you can focus on your business rules instead of rewriting infrastructure-independent domain code.

Benefits include:

- Clean Domain-Driven Design foundation
- Infrastructure-independent architecture
- Reusable domain building blocks
- Consistent modeling across projects
- Lightweight and dependency-free

---

# Features

- ✅ `BaseEntity<TId>`
- ✅ `AggregateRoot<TId>`
- ✅ `ValueObject`
- ✅ Domain Events
- ✅ Guard Clauses
- ✅ Repository Interfaces
- ✅ Unit of Work Interface
- ✅ Specification Pattern
- ✅ Common Domain Value Objects
- ✅ Pagination Model

---

# BaseEntity<TId>

Base class for all entities.

Features:

- Entity identity
- Equality support
- Hash code implementation

Example:

```csharp
public class Product : BaseEntity<Guid>
{
    public Product(Guid id) : base(id)
    {
    }
}
```

---

# AggregateRoot<TId>

Base class for aggregate roots.

Features:

- Domain event support
- Entity identity
- Aggregate behavior

Example:

```csharp
public class Order : AggregateRoot<Guid>
{
    // Domain logic
}
```

---

# ValueObject

Simplifies creating immutable value objects with value-based equality.

Example:

```csharp
public class Money : ValueObject
{
    public decimal Amount { get; }

    public string Currency { get; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}
```

---

# Guard Clauses

Validate input values using expressive guard methods.

Available methods:

- `AgainstNull()`
- `AgainstNullOrWhiteSpace()`
- `AgainstOutOfRange()`

Example:

```csharp
Name = Guard.AgainstNullOrWhiteSpace(name, nameof(name));

Price = Guard.AgainstNull(price, nameof(price));
```

---

# Repository Abstractions

Infrastructure-independent repository interfaces.

Available interfaces:

- `IRepository<T>`
- `IReadOnlyRepository<T>`
- `IUnitOfWork`

These interfaces allow your domain layer to remain completely independent from persistence technologies.

---

# Specification Pattern

Supports reusable business queries using the Specification Pattern.

Available types:

- `ISpecification<T>`
- `BaseSpecification<T>`

Example:

```csharp
public class ActiveProductsSpecification
    : BaseSpecification<Product>
{
    public ActiveProductsSpecification()
    {
        Criteria = x => x.IsActive;
    }
}
```

---

# Built-in Domain Models

The package includes several reusable domain models:

- `EmailAddress`
- `PhoneNumber`
- `Money`
- `Address`
- `DateRange`
- `PagedResult<T>`

These models encapsulate common business concepts and promote consistency throughout your domain.

---

# Domain Events

Built-in support for domain events.

Available types:

- `IDomainEvent`
- `DomainEventBase`
- `IEventHandler<T>`

Example:

```csharp
public class ProductCreatedEvent : DomainEventBase
{
    public Guid ProductId { get; }

    public ProductCreatedEvent(Guid productId)
    {
        ProductId = productId;
    }
}
```

---

# Complete Example

```csharp
using ImanSoftware.Domain;

public class Product : BaseEntity<Guid>
{
    public string Name { get; private set; }

    public Money Price { get; private set; }

    public Product(Guid id, string name, Money price)
        : base(id)
    {
        Name = Guard.AgainstNullOrWhiteSpace(name, nameof(name));

        Price = Guard.AgainstNull(price, nameof(price));
    }
}
```

---

# Design Goals

- Domain-Driven Design first
- Infrastructure independent
- Clean Architecture friendly
- High performance
- Lightweight
- Minimal dependencies
- Reusable domain abstractions
- Rich domain modeling
- Easy to extend

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