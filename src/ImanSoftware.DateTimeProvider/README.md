# ImanSoftware.DateTimeProvider

> A lightweight abstraction over `System.DateTime` for clean, testable, and dependency-injected date and time access.

`ImanSoftware.DateTimeProvider` eliminates direct dependencies on `DateTime.Now` and `DateTime.Today` by providing a simple, injectable abstraction. This makes your applications easier to test, maintain, and extend.

---

## Installation

Install the package using the .NET CLI:

```bash
dotnet add package ImanSoftware.DateTimeProvider
```

---

## Why DateTimeProvider?

Using `DateTime.Now` directly introduces hidden dependencies that make unit testing difficult.

Instead of:

```csharp
var createdAt = DateTime.Now;
```

Use:

```csharp
var createdAt = _dateTimeProvider.Now;
```

Your code becomes deterministic and can easily be tested by mocking the current time.

Benefits include:

- Improved testability
- Dependency Injection friendly
- Clean Architecture compatible
- Centralized time access
- Lightweight and dependency-free

---

# Features

- ✅ `IDateTimeProvider` abstraction
- ✅ `Now` property
- ✅ `Today` property
- ✅ `CurrentTimeOfDay` property
- ✅ Default implementation using `System.DateTime`
- ✅ Easy Dependency Injection registration
- ✅ Mock-friendly design
- ✅ Zero configuration

---

# IDateTimeProvider

The core abstraction exposes commonly used date and time values.

Available members:

- `Now`
- `Today`
- `CurrentTimeOfDay`

Example:

```csharp
public interface IDateTimeProvider
{
    DateTime Now { get; }

    DateTime Today { get; }

    TimeSpan CurrentTimeOfDay { get; }
}
```

---

# DefaultDateTimeProvider

The package includes a default implementation that wraps `System.DateTime`.

```csharp
var provider = new DefaultDateTimeProvider();

var now = provider.Now;

var today = provider.Today;

var time = provider.CurrentTimeOfDay;
```

---

# Dependency Injection

Register the provider in your application:

```csharp
services.AddDateTimeProvider();
```

After registration, `IDateTimeProvider` can be injected anywhere in your application.

---

# Example

```csharp
using ImanSoftware.DateTimeProvider;

public class OrderService
{
    private readonly IDateTimeProvider _dateTime;

    public OrderService(IDateTimeProvider dateTime)
    {
        _dateTime = dateTime;
    }

    public void CreateOrder()
    {
        var now = _dateTime.Now;

        // Business logic...
    }
}
```

---

# Unit Testing

Because the provider is interface-based, replacing it with a mock or fake implementation is straightforward.

```csharp
var mock = new Mock<IDateTimeProvider>();

mock.Setup(x => x.Now)
    .Returns(new DateTime(2026, 1, 1));
```

Your business logic can now be tested without relying on the system clock.

---

# Design Goals

- Clean abstraction
- High testability
- Dependency Injection friendly
- Lightweight
- Framework independent
- Minimal API
- Zero configuration
- Easy integration
- Clean Architecture compatible

---

# Requirements

- .NET Standard 2.0+
- .NET 6+
- .NET 7+
- .NET 8+
- .NET 9+

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