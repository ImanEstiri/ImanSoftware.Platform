# ImanSoftware.CurrentUserAccessor

**Version:** 1.0.0 | **Last Updated:** August 2026

> A lightweight abstraction for accessing the currently authenticated user in ASP.NET Core with support for strongly typed user identifiers.

`ImanSoftware.CurrentUserAccessor` provides a clean and testable way to access the authenticated user without depending directly on `IHttpContextAccessor` throughout your application. It supports automatic conversion of the `sub` claim into strongly typed user IDs such as `Guid`, `int`, `long`, `string`, and other primitive types.

---

## Installation

Install the package using the .NET CLI:

```bash
dotnet add package ImanSoftware.CurrentUserAccessor
```

---

## Why CurrentUserAccessor?

Accessing `HttpContext.User` directly spreads infrastructure concerns across your application and makes unit testing more difficult.

Instead of:

```csharp
var userId = Guid.Parse(
    HttpContext.User.FindFirst("sub")!.Value);
```

Use:

```csharp
var userId = _currentUserAccessor.GetCurrentUserId();
```

This keeps your business logic clean while making it easy to mock the authenticated user during testing.

Benefits include:

- Strongly typed user identifiers
- Improved testability
- Clean Architecture friendly
- Dependency Injection compatible
- Centralized user access

---

# Features

- ✅ `ICurrentUserAccessor<TUserId>`
- ✅ Strongly typed user IDs
- ✅ Access to `ClaimsPrincipal`
- ✅ Automatic `sub` claim conversion
- ✅ Supports `Guid`, `int`, `long`, `string`, and other primitive types
- ✅ Built on `IHttpContextAccessor`
- ✅ Open Generic Dependency Injection registration
- ✅ Lightweight and extensible

---

# ICurrentUserAccessor<TUserId>

The primary abstraction for accessing the authenticated user.

Available methods:

- `GetCurrentUserId()`
- `GetCurrentUser()`

Example:

```csharp
Guid userId = _currentUserAccessor.GetCurrentUserId();

ClaimsPrincipal user = _currentUserAccessor.GetCurrentUser();
```

---

# HttpContextUserAccessor<TUserId>

The default implementation uses `IHttpContextAccessor` to retrieve the current authenticated user.

Features include:

- Reads the authenticated `ClaimsPrincipal`
- Extracts the `sub` claim
- Automatically converts it to `TUserId`
- Supports custom user identifier types through `TypeDescriptor`

---

# Supported User ID Types

The package supports any type that can be converted through `TypeDescriptor`.

Common examples include:

- `Guid`
- `string`
- `int`
- `long`
- `short`
- `byte`
- Other primitive types

Example:

```csharp
ICurrentUserAccessor<Guid>

ICurrentUserAccessor<int>

ICurrentUserAccessor<long>

ICurrentUserAccessor<string>
```

---

# Dependency Injection

Register the accessor in your application:

```csharp
services.AddCurrentUserAccessor();
```

The service is registered as an **Open Generic**, allowing it to be injected using any supported user ID type.

> **Note**
>
> If you are using **AddImanFramework()**, the current user accessor is registered automatically.

---

# Example

```csharp
using ImanSoftware.CurrentUserAccessor;

public class ProfileService
{
    private readonly ICurrentUserAccessor<Guid> _userAccessor;

    public ProfileService(
        ICurrentUserAccessor<Guid> userAccessor)
    {
        _userAccessor = userAccessor;
    }

    public void ShowProfile()
    {
        Guid userId = _userAccessor.GetCurrentUserId();

        ClaimsPrincipal user = _userAccessor.GetCurrentUser();

        // Business logic...
    }
}
```

---

# Unit Testing

Because user access is abstracted behind an interface, it can easily be mocked.

```csharp
var mock = new Mock<ICurrentUserAccessor<Guid>>();

mock.Setup(x => x.GetCurrentUserId())
    .Returns(Guid.Parse(
        "11111111-1111-1111-1111-111111111111"));
```

This allows your business logic to be tested without an active HTTP request.

---

# Design Goals

- Strongly typed API
- Clean abstraction
- Dependency Injection friendly
- Easy to mock
- Lightweight
- Framework independent
- Clean Architecture compatible
- ASP.NET Core friendly
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