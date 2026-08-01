# ImanSoftware.Outcome

> A lightweight and extensible **Result/Outcome** implementation for .NET that helps you manage application flow without relying on exceptions.

`ImanSoftware.Outcome` provides a clean implementation of the **Result Pattern**, allowing methods to return success or failure states with meaningful error information instead of throwing exceptions for expected scenarios.

---

## Installation

Install the package using the .NET CLI:

```bash
dotnet add package ImanSoftware.Outcome
```

---

## Why Outcome?

Using exceptions for expected business scenarios often makes code harder to read, test, and maintain.

Instead of this:

```csharp
try
{
    var product = service.Get(id);
}
catch (NotFoundException)
{
    // Handle error
}
```

You can write:

```csharp
var result = service.Get(id);

if (!result.IsSuccess)
{
    // Handle error
}
```

This approach provides:

- Better readability
- Explicit success/failure flow
- Easier testing
- Better performance by avoiding unnecessary exceptions
- Consistent error handling across projects

---

# Features

- ✅ `Outcome` and `Outcome<T>` types
- ✅ Success and Failure states
- ✅ Rich `OutcomeError` object
- ✅ Predefined error types
- ✅ Implicit conversion from `T` to `Outcome<T>`
- ✅ Common application exceptions
- ✅ Convert exceptions to `Outcome` using `ToResult()`
- ✅ Lightweight and dependency-free

---

# Outcome Types

## Outcome

Represents an operation without a return value.

```csharp
Outcome.Successful();

Outcome.Failure(
    new OutcomeError(
        "Something went wrong",
        "GENERAL_ERROR",
        OutcomeErrorType.Failure));
```

---

## Outcome<T>

Represents an operation that returns data.

```csharp
Outcome<Product>.Successful(product);
```

or simply:

```csharp
Product product = GetProduct();

Outcome<Product> result = product;
```

---

# OutcomeError

Each failure contains an `OutcomeError`.

```csharp
new OutcomeError(
    message: "Product not found",
    code: "NOT_FOUND",
    type: OutcomeErrorType.NotFound);
```

Properties:

| Property | Description |
|----------|-------------|
| Message | Human-readable error message |
| Code | Application-specific error code |
| Type | Error category |

---

# Error Types

The package includes predefined error categories:

| Type | Description |
|------|-------------|
| None | No error |
| Failure | General failure |
| Validation | Validation error |
| Problem | Unexpected problem |
| NotFound | Resource not found |
| Conflict | Conflict with current state |
| InquiryError | Business inquiry error |

---

# Built-in Exceptions

The package includes common exceptions used in business applications.

- `NotFoundException`
- `ValidationException`
- `BusinessRuleException`
- `UnauthorizedException`
- `ForbiddenException`
- `ConflictException`
- `BadRequestException`

These exceptions can easily be converted into an `Outcome`.

---

# Exception to Outcome

```csharp
try
{
    // code
}
catch (Exception ex)
{
    return ex.ToResult();
}
```

---

# Example

```csharp
using ImanSoftware.Outcome;

public Outcome<Product> GetProduct(int id)
{
    var product = _repository.GetById(id);

    if (product is null)
    {
        return Outcome<Product>.Failure(
            new OutcomeError(
                "Product not found",
                "NOT_FOUND",
                OutcomeErrorType.NotFound));
    }

    return Outcome<Product>.Successful(product);
}
```

---

# Working with Results

```csharp
var result = GetProduct(id);

if (result.IsSuccess)
{
    Console.WriteLine(result.Value.Name);
}
else
{
    Console.WriteLine(result.Error.Message);
}
```

---

# Design Goals

- Simple API
- High performance
- Minimal allocations
- Framework independent
- Easy integration with ASP.NET Core
- Suitable for Clean Architecture
- Suitable for DDD
- Suitable for CQRS
- Consistent error handling

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