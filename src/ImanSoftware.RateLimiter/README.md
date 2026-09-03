# 🚦 ImanSoftware.RateLimiter

**Version:** 1.0.3 | **Last Updated:** August 2026

**A lightweight rate limiting library for ASP.NET Core applications.**

---

## 📖 Table of Contents

- [Introduction](#-introduction)
- [Features](#-features)
- [Installation](#-installation)
- [Usage](#-usage)
- [Custom Strategy](#-custom-strategy)
- [Supported Strategies](#-supported-strategies)
- [Storage](#-storage)
- [ASP.NET Core Middleware](#-aspnet-core-middleware)
- [Configuration](#-configuration)
- [License](#-license)

---

## 🎯 Introduction

**ImanSoftware.RateLimiter** is a lightweight rate limiting library for ASP.NET Core applications.

It provides configurable rate limiting policies and supports multiple rate limiting strategies through ASP.NET Core middleware.

---

## ✨ Features

- ✅ **Fixed Window** strategy
- ✅ **Sliding Window** strategy
- ✅ **Token Bucket** strategy
- ✅ **In-memory storage**
- 🚧 **Redis support** — coming soon
- ✅ **ASP.NET Core middleware**
- ✅ **Highly configurable policies**
- ✅ **Extensible strategies**

---

## 📦 Installation

Install the package via NuGet Package Manager:

```bash
dotnet add package ImanSoftware.RateLimiter
```

---

## 🚀 Usage

### Configure Rate Limiting

Register the rate limiter in `Program.cs`:

```csharp
builder.Services.AddRateLimiter(options =>
{
    options.DefaultStrategy = "FixedWindow";

    options.DefaultPolicy = new RateLimitPolicy
    {
        Limit = 100,
        WindowSeconds = 60
    };

    options.Policies["api"] = new RateLimitPolicy
    {
        Limit = 50,
        WindowSeconds = 10
    };
});
```

Then enable the middleware:

```csharp
app.UseRateLimiter();
```

---

## 🧩 Custom Strategy

The library is extensible and allows custom rate limiting strategies to be registered.

```csharp
builder.Services.AddRateLimiter(
    configureOptions: options =>
    {
        // Configure options...
    },
    customRegistration: services =>
    {
        services.TryAddSingleton<IRateLimiterStrategy, TokenBucketStrategy>();
    }
);
```

This allows applications to provide their own strategy implementation or customize strategy registration through dependency injection.

---

## 🔧 Supported Strategies

The package currently supports:

| Strategy | Status |
|---|---|
| Fixed Window | ✅ Supported |
| Sliding Window | ✅ Supported |
| Token Bucket | ✅ Supported |

### Fixed Window

Limits requests within fixed time windows.

### Sliding Window

Provides rate limiting based on a rolling time window.

### Token Bucket

Uses a token-based mechanism to control request rates.

---

## 💾 Storage

The current implementation uses **in-memory storage**.

Redis-based storage is planned for a future release.

---

## 🌐 ASP.NET Core Middleware

The library includes middleware for applying rate limiting to ASP.NET Core applications.

Enable it with:

```csharp
app.UseRateLimiter();
```

The middleware works with the configured rate limiting policies and strategies.

---

## ⚙️ Configuration

A default strategy and default policy can be configured:

```csharp
options.DefaultStrategy = "FixedWindow";

options.DefaultPolicy = new RateLimitPolicy
{
    Limit = 100,
    WindowSeconds = 60
};
```

Additional named policies can be configured:

```csharp
options.Policies["api"] = new RateLimitPolicy
{
    Limit = 50,
    WindowSeconds = 10
};
```

This allows different parts of an application to use different rate limiting policies.

---

# Design Goals

- Clean abstraction
- Dependency Injection friendly
- High performance
- Structured logging support
- Scope support
- Easy to mock
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