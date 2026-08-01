# ImanSoftware.DataAccess.Dapper

**Version:** 1.0.0 | **Last Updated:** August 2026

> A high-performance Dapper implementation for **ImanSoftware.DataAccess**, providing fast, lightweight, and efficient SQL data access for .NET applications.

`ImanSoftware.DataAccess.Dapper` implements the abstractions defined in **ImanSoftware.DataAccess** using **Dapper** and **Microsoft.Data.SqlClient**. It offers efficient query execution, automatic connection management, transaction support, and seamless integration with Dependency Injection.

---

## Installation

Install the package using the .NET CLI:

```bash
dotnet add package ImanSoftware.DataAccess.Dapper
```

---

## Why ImanSoftware.DataAccess.Dapper?

Dapper is one of the fastest micro-ORMs available for .NET, but using it directly often results in repeated connection handling, transaction management, and boilerplate code.

This package combines Dapper's performance with the clean abstractions provided by **ImanSoftware.DataAccess**.

Benefits include:

- High-performance data access
- Minimal overhead
- Clean Architecture compatible
- Dependency Injection friendly
- Consistent `Outcome`-based error handling
- Automatic connection management

---

# Features

- ✅ `DapperDataAccessor`
- ✅ `DapperConnectionFactory`
- ✅ `DapperTransactionScope`
- ✅ Automatic connection management
- ✅ Stored Procedure support
- ✅ Parameterized SQL queries
- ✅ Custom mapping support
- ✅ Async-first API
- ✅ Transaction support
- ✅ Full integration with `ImanSoftware.DataAccess`

---

# DapperDataAccessor

Implements the `ISqlDataAccessor` abstraction using Dapper.

Supported operations:

- `QuerySingleAsync<T>()`
- `QueryAsync<T>()`
- `ExecuteAsync()`
- `ExecuteScalarAsync<T>()`
- `BeginTransactionAsync()`

Example:

```csharp
var result = await _db.QuerySingleAsync<Product>(
    "GetProductById",
    new
    {
        Id = id
    },
    CommandType.StoredProcedure);
```

All operations return `Outcome` objects.

---

# DapperConnectionFactory

Creates SQL Server connections using **Microsoft.Data.SqlClient**.

Responsibilities include:

- Connection creation
- Connection lifetime management
- Provider abstraction

---

# DapperTransactionScope

Provides transaction management on top of Dapper.

Supported operations:

- `CommitAsync()`
- `RollbackAsync()`

Example:

```csharp
await using var transaction =
    await _db.BeginTransactionAsync();

await _db.ExecuteAsync(...);

await transaction.CommitAsync();
```

---

# Automatic Connection Management

Connections are automatically:

- Opened when required
- Reused during transactions
- Closed when operations complete
- Properly disposed

This keeps application code clean while preventing connection leaks.

---

# Custom Mapping

The package supports Dapper's custom mapping capabilities, allowing database schemas to be mapped cleanly to domain models.

Example scenarios include:

- Different column names
- Complex object mapping
- Legacy database integration

---

# Dependency Injection

Register the Dapper implementation:

```csharp
services.AddDapperDataAccess(connectionString);
```

Alternatively, register it through the generic Data Access configuration.

```csharp
services.AddDataAccess(connectionString);
```

---

# Example

```csharp
using ImanSoftware.DataAccess.Dapper;

public class ProductService
{
    private readonly ISqlDataAccessor _db;

    public ProductService(ISqlDataAccessor db)
    {
        _db = db;
    }

    public async Task<Outcome<int>> CreateProductAsync(Product product)
    {
        var parameters = new
        {
            product.Name,
            product.Price
        };

        return await _db.ExecuteAsync(
            "InsertProduct",
            parameters,
            CommandType.StoredProcedure);
    }
}
```

---

# Stored Procedure Example

```csharp
await _db.ExecuteAsync(
    "InsertProduct",
    new
    {
        product.Name,
        product.Price
    },
    CommandType.StoredProcedure);
```

---

# Ad-hoc SQL Example

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

# Design Goals

- High performance
- Thin abstraction over Dapper
- Async-first API
- Dependency Injection friendly
- Automatic connection management
- Transaction support
- Lightweight
- Outcome-based error handling
- Clean Architecture compatible

---

# Requirements

- .NET Standard 2.0+
- .NET 6+
- .NET 7+
- .NET 8+
- .NET 9+
- .NET 10.0+

---

# Dependencies

- **ImanSoftware.DataAccess**
- **ImanSoftware.Outcome**
- **Dapper**
- **Microsoft.Data.SqlClient**

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