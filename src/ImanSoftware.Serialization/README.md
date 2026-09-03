# ImanSoftware.Serialization

**Version:** 1.0.2 | **Last Updated:** August 2026

> A flexible JSON serialization abstraction for .NET with support for both **System.Text.Json** and **Newtonsoft.Json**.

`ImanSoftware.Serialization` provides a unified interface for JSON serialization and deserialization, allowing applications to switch between serialization engines without changing business logic. Choose the implementation that best fits your project while keeping your code clean and testable.

---

## Installation

Install the package using the .NET CLI:

```bash
dotnet add package ImanSoftware.Serialization
```

---

## Why Serialization Abstraction?

Tightly coupling your application to a specific JSON library makes future migrations and testing more difficult.

Instead of:

```csharp
JsonSerializer.Serialize(product);
```

or

```csharp
JsonConvert.SerializeObject(product);
```

Use:

```csharp
_jsonSerializer.Serialize(product);
```

This allows the serialization engine to be configured centrally through Dependency Injection.

Benefits include:

- Easily switch between JSON libraries
- Improved testability
- Cleaner architecture
- Dependency Injection friendly
- Consistent serialization API

---

# Features

- ✅ `IJsonSerializer` abstraction
- ✅ JSON serialization
- ✅ JSON deserialization
- ✅ `System.Text.Json` implementation
- ✅ `Newtonsoft.Json` implementation
- ✅ Configurable serializer options
- ✅ Dependency Injection support
- ✅ Easy implementation switching

---

# IJsonSerializer

The core abstraction provides serialization and deserialization methods.

```csharp
public interface IJsonSerializer
{
    string Serialize(object value);

    T? Deserialize<T>(string json);
}
```

---

# SystemTextJsonSerializer

Default implementation based on `System.Text.Json`.

Features:

- High performance
- Low memory allocations
- Configurable `JsonSerializerOptions`
- Built into modern .NET

Example:

```csharp
services.AddJsonSerializer();
```

---

# NewtonsoftJsonSerializer

Implementation based on `Newtonsoft.Json`.

Features:

- Mature and feature-rich
- Extensive customization
- Configurable `JsonSerializerSettings`
- Suitable for legacy and advanced scenarios

Example:

```csharp
services.AddJsonSerializer(
    SerializationDefaultImplementationType.Newtonsoft);
```

---

# Dependency Injection

Register the default serializer:

```csharp
services.AddJsonSerializer();
```

This registers:

```text
System.Text.Json
```

To use Newtonsoft.Json instead:

```csharp
services.AddJsonSerializer(
    SerializationDefaultImplementationType.Newtonsoft);
```

---

# Example

```csharp
using ImanSoftware.Serialization;

public class ProductService
{
    private readonly IJsonSerializer _json;

    public ProductService(IJsonSerializer json)
    {
        _json = json;
    }

    public string Serialize(Product product)
    {
        return _json.Serialize(product);
    }

    public Product? Deserialize(string json)
    {
        return _json.Deserialize<Product>(json);
    }
}
```

---

# Unit Testing

Because serialization is abstracted behind an interface, it can easily be mocked.

```csharp
var mock = new Mock<IJsonSerializer>();

mock.Setup(x => x.Serialize(It.IsAny<Product>()))
    .Returns("{\"id\":1}");
```

This keeps tests isolated from any specific serialization library.

---

# Design Goals

- Serializer-agnostic
- High performance
- Dependency Injection friendly
- Clean Architecture compatible
- Easy to configure
- Lightweight
- Mock-friendly
- Extensible
- Consistent API

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