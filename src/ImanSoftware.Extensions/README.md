# ImanSoftware.Extensions

**Version:** 1.0.1 | **Last Updated:** August 2026

> A lightweight collection of useful .NET extension methods that reduce boilerplate code and simplify everyday development.

`ImanSoftware.Extensions` provides a rich set of extension methods for the most commonly used .NET types, making your code cleaner, more expressive, and easier to maintain.

---

## Installation

Install the package using the .NET CLI:

```bash
dotnet add package ImanSoftware.Extensions
```

---

## Why ImanSoftware.Extensions?

Every .NET project ends up implementing the same helper methods over and over again.

This package centralizes those common extensions into a single, dependency-free library.

Benefits include:

- Less repetitive code
- Improved readability
- Cleaner APIs
- Consistent helper methods across projects
- Lightweight and easy to use

---

# Features

## String Extensions

Useful helpers for working with strings.

Available methods:

- `ToCamelCase()`
- `ToSnakeCase()`
- `Truncate()`
- `IsEmail()`
- `ExtractNumbers()`
- `NormalizeWhitespace()`
- `IsNullOrWhiteSpace()`

Example:

```csharp
using ImanSoftware.Extensions;

var text = "  hello    world  ";

var normalized = text.NormalizeWhitespace();
// hello world

var camel = text.ToCamelCase();
// helloWorld

var snake = text.ToSnakeCase();
// hello_world

var numbers = "Phone: +1 (234) 567-890".ExtractNumbers();
// 1234567890
```

---

# DateTime Extensions

Utility methods for working with dates and times.

Available methods:

- `StartOfDay()`
- `EndOfDay()`
- `StartOfWeek()`
- `Age()`
- `IsToday()`
- `ToUnixTimestamp()`
- `FromUnixTimestamp()`

Example:

```csharp
using ImanSoftware.Extensions;

var now = DateTime.Now;

var start = now.StartOfDay();
var end = now.EndOfDay();
var week = now.StartOfWeek();

var age = birthDate.Age();

bool isToday = now.IsToday();

long unix = now.ToUnixTimestamp();

DateTime date = DateTimeExtensions.FromUnixTimestamp(unix);
```

---

# Enum Extensions

Additional functionality for enumerations.

Available methods:

- `GetDescription()`
- `GetDisplayName()`
- `GetValues()`
- `ToInt()`
- `Parse()`

Example:

```csharp
var description = OrderStatus.Completed.GetDescription();

int value = OrderStatus.Completed.ToInt();

var status = EnumExtensions.Parse<OrderStatus>("Completed");
```

---

# Enumerable Extensions

Helpful extensions for collections.

Available methods:

- `IsNullOrEmpty()`
- `DistinctBy()`
- `Chunk()`
- `ForEach()`
- `ToHashSet()`

Example:

```csharp
var users = repository.GetUsers();

if (!users.IsNullOrEmpty())
{
    users.ForEach(Console.WriteLine);
}

var unique = users.DistinctBy(x => x.Id);

var groups = users.Chunk(10);

var hashSet = users.ToHashSet();
```

---

# Object Extensions

General-purpose object helpers.

Available methods:

- `ToJson()`
- `IsNull()`
- `ThrowIfNull()`
- `DeepCopy()`

Example:

```csharp
var json = user.ToJson();

user.ThrowIfNull();

var clone = user.DeepCopy();

bool isNull = user.IsNull();
```

---

# Exception Extensions

Utilities for working with exceptions.

Available methods:

- `GetFullMessage()`
- `GetFullStackTrace()`
- `Is<T>()`
- `GetInnermostException()`

Example:

```csharp
try
{
    // Code...
}
catch (Exception ex)
{
    Console.WriteLine(ex.GetFullMessage());

    Console.WriteLine(ex.GetFullStackTrace());

    if (ex.Is<ArgumentException>())
    {
        // Handle argument exception
    }

    var inner = ex.GetInnermostException();
}
```

---

# Example

```csharp
using ImanSoftware.Extensions;

var name = "  hello world  ";

var normalized = name.NormalizeWhitespace();
// hello world

var camel = name.ToCamelCase();
// helloWorld

var snake = name.ToSnakeCase();
// hello_world

var date = DateTime.Now;

var startOfWeek = date.StartOfWeek();

var age = birthDate.Age();

long unix = date.ToUnixTimestamp();
```

---

# Design Goals

- Lightweight
- Dependency-free
- High performance
- Simple API
- Clean and consistent naming
- Everyday developer utilities
- Compatible with modern .NET applications

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