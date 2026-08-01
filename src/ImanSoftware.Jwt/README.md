# ImanSoftware.Jwt

> A lightweight and extensible JWT authentication library for .NET with token generation, validation, and ASP.NET Core middleware support.

`ImanSoftware.Jwt` provides a clean abstraction over JWT operations, making it easy to generate, validate, and process JSON Web Tokens while integrating seamlessly with ASP.NET Core applications.

---

## Installation

Install the package using the .NET CLI:

```bash
dotnet add package ImanSoftware.Jwt
```

---

## Why ImanSoftware.Jwt?

Working directly with the JWT APIs often leads to duplicated configuration and authentication logic across projects.

Instead of scattering JWT-related code throughout your application, this package centralizes token generation, validation, and authentication into a single, reusable service.

Benefits include:

- Simplified JWT management
- Dependency Injection friendly
- Clean Architecture compatible
- Easy integration with ASP.NET Core
- Lightweight and testable

---

# Features

- ✅ `IJwtTokenService` abstraction
- ✅ JWT generation
- ✅ JWT validation
- ✅ Read claims from expired tokens
- ✅ Configurable `JwtOptions`
- ✅ ASP.NET Core authentication middleware
- ✅ Dependency Injection support
- ✅ Easy application setup

---

# IJwtTokenService

The primary abstraction for working with JWT tokens.

Available methods:

- `GenerateToken()`
- `ValidateToken()`
- `GetPrincipalFromExpiredToken()`

Example:

```csharp
string token = _jwt.GenerateToken(claims);

bool isValid = _jwt.ValidateToken(token);

var principal = _jwt.GetPrincipalFromExpiredToken(token);
```

---

# JwtTokenService

The default implementation is built on top of `System.IdentityModel.Tokens.Jwt`.

Features include:

- Secure JWT generation
- Token validation
- Claims extraction
- Expired token support (useful for refresh token scenarios)
- Standards-compliant implementation

---

# JwtOptions

JWT behavior is configured through the `JwtOptions` class.

Available settings:

| Property | Description |
|----------|-------------|
| `SecretKey` | Secret key used to sign tokens |
| `Issuer` | Token issuer |
| `Audience` | Token audience |
| `ExpiryMinutes` | Token expiration time |
| `ValidateLifetime` | Enables or disables lifetime validation |

Example configuration:

```json
{
  "Jwt": {
    "SecretKey": "your-secret-key",
    "Issuer": "MyApplication",
    "Audience": "MyApplication",
    "ExpiryMinutes": 60,
    "ValidateLifetime": true
  }
}
```

---

# Dependency Injection

Register the JWT service using your application configuration.

```csharp
var jwtOptions = builder.Configuration
    .GetSection("Jwt")
    .Get<JwtOptions>();

services.AddJwtTokenService(jwtOptions);
```

After registration, `IJwtTokenService` can be injected anywhere in your application.

---

# JWT Middleware

The package includes a dedicated middleware that:

- Reads the JWT from the `Authorization` header
- Validates the token
- Creates the authenticated `ClaimsPrincipal`
- Assigns the user to `HttpContext.User`

Register the middleware in your request pipeline:

```csharp
app.UseJwtAuthentication();
```

---

# Example

```csharp
using ImanSoftware.Jwt;

public class AuthService
{
    private readonly IJwtTokenService _jwt;

    public AuthService(IJwtTokenService jwt)
    {
        _jwt = jwt;
    }

    public string Login(string username, string password)
    {
        // Validate user credentials...

        var claims = new[]
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                userId.ToString())
        };

        return _jwt.GenerateToken(claims);
    }
}
```

---

# Complete Setup

```csharp
var jwtOptions = builder.Configuration
    .GetSection("Jwt")
    .Get<JwtOptions>();

services.AddJwtTokenService(jwtOptions);

var app = builder.Build();

app.UseJwtAuthentication();
```

---

# Common Use Cases

- User authentication
- API authorization
- Refresh token workflows
- Claims-based identity
- Microservices authentication
- Stateless security

---

# Design Goals

- Secure by default
- Clean abstraction
- Dependency Injection friendly
- Lightweight
- Easy configuration
- ASP.NET Core integration
- Standards compliant
- Clean Architecture compatible
- Easy to test

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