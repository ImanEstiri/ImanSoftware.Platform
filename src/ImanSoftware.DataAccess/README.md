# ImanSoftware.DataAccess

> A lightweight and extensible data access abstraction for .NET with support for parameterized queries, stored procedures, transactions, and `Outcome`-based results.

`ImanSoftware.DataAccess` provides a clean abstraction over SQL data access, allowing applications to execute queries and manage transactions without being tightly coupled to a specific implementation. All operations return `Outcome` objects, enabling consistent error handling without relying on exceptions for expected failures.

---

## Installation

Install the package using the .NET CLI:

```bash
dotnet add package ImanSoftware.DataAccess
```

---

## Why ImanSoftware.DataAccess?

Writing data access code often leads to repetitive connection management, transaction handling, and exception handling.

This package centralizes those concerns behind a simple abstraction while integrating seamlessly with **ImanSoftware.Outcome** for explicit success and failure handling.

Benefits include:

- Clean Architecture friendly
- Dependency Injection compatible
- Transaction support
- Parameterized SQL execution
- Stored Procedure support
- Consistent `Outcome`-based error handling

---

# Features

- ✅ `ISqlDataAccessor`
- ✅ `IDbConnectionFactory`
- ✅ `ITransactionScope`
- ✅ Async API
- ✅ Parameterized queries
- ✅ Stored Procedure support
- ✅ Ad-hoc SQL support
- ✅ Transaction management
- ✅ `Outcome`-based results
- ✅ Dependency Injection support

---

# ISqlDataAccessor

The primary abstraction for executing SQL commands.

Available methods:

- `QuerySingleAsync<T>()`
- `QueryAsync<T>()`
- `ExecuteAsync()`
- `ExecuteScalarAsync<T>()`
- `BeginTransactionAsync()`

Example:

```csharp
var result = await _db.QuerySingleAsync<Product>(
    "GetProductById",
    new { Id = id },
    CommandType.StoredProcedure);
```

All methods return `Outcome` objects, allowing failures to be handled explicitly.

---

# IDbConnectionFactory

Responsible for creating database connections.

This abstraction keeps your application independent from any specific database provider and centralizes connection creation.

---

# ITransactionScope

Provides an abstraction for database transactions.

Available methods:

- `CommitAsync()`
- `RollbackAsync()`

Example:

```csharp
await using var transaction =
    await _db.BeginTransactionAsync();

await _db.ExecuteAsync(...);

await transaction.CommitAsync();
```

If an error occurs:

```csharp
await transaction.RollbackAsync();
```

---

# Stored Procedures

Execute stored procedures easily.

```csharp
await _db.QuerySingleAsync<Product>(
    "GetProductById",
    new
    {
        Id = id
    },
    CommandType.StoredProcedure);
```

---

# Ad-hoc SQL Queries

Parameterized SQL queries are also supported.

```csharp
await _db.QueryAsync<Product>(
    @"SELECT *
      FROM Products
      WHERE CategoryId = @CategoryId",
    new
    {
        CategoryId = categoryId
    });
```

---

# Outcome-Based Results

Every database operation returns an `Outcome`.

Example:

```csharp
var result = await _db.QuerySingleAsync<Product>(...);

if (result.IsSuccess)
{
    Product product = result.Value;
}
else
{
    Console.WriteLine(result.Error.Message);
}
```

This provides predictable error handling without relying on exceptions for expected scenarios.

---

# Dependency Injection

Register the data access services:

```csharp
services.AddDataAccess(connectionString);
```

After registration, `ISqlDataAccessor` can be injected anywhere in your application.

---

# Example

```csharp
using ImanSoftware.DataAccess;

public class ProductService
{
    private readonly ISqlDataAccessor _db;

    public ProductService(ISqlDataAccessor db)
    {
        _db = db;
    }

    public async Task<Outcome<Product>> GetProductAsync(int id)
    {
        return await _db.QuerySingleAsync<Product>(
            "GetProductById",
            new
            {
                Id = id
            },
            CommandType.StoredProcedure);
    }
}
```

---

# Transaction Example

```csharp
await using var transaction =
    await _db.BeginTransactionAsync();

await _db.ExecuteAsync(...);

await _db.ExecuteAsync(...);

await transaction.CommitAsync();
```

---

# Design Goals

- Clean abstraction
- Async-first API
- Dependency Injection friendly
- Transaction support
- Provider independent
- Lightweight
- High performance
- Outcome-based error handling
- Clean Architecture compatible

---

# Requirements

- .NET Standard 2.0+
- .NET 6+
- .NET 7+
- .NET 8+
- .NET 9+

---

# Dependencies

- **ImanSoftware.Outcome**

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