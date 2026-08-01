# ImanSoftware.FileStorage

> A lightweight and extensible file storage abstraction for .NET with a built-in local file system implementation.

`ImanSoftware.FileStorage` provides a unified API for managing files regardless of the underlying storage provider. It supports uploading, downloading, deleting, moving, copying, and retrieving file metadata while returning `Outcome` objects for consistent error handling.

---

## Installation

Install the package using the .NET CLI:

```bash
dotnet add package ImanSoftware.FileStorage
```

---

## Why ImanSoftware.FileStorage?

Working directly with `System.IO` often results in duplicated file management logic and tightly couples applications to the local file system.

`ImanSoftware.FileStorage` abstracts storage operations behind a clean interface, making it easy to switch storage providers in the future while keeping your business logic unchanged.

Benefits include:

- Storage provider abstraction
- Dependency Injection friendly
- Consistent `Outcome`-based error handling
- Easy unit testing
- Extensible architecture

---

# Features

- ✅ File upload
- ✅ File download
- ✅ File deletion
- ✅ File move
- ✅ File copy
- ✅ File metadata retrieval
- ✅ Local file system implementation
- ✅ Configurable root directory
- ✅ Dependency Injection support
- ✅ `Outcome`-based API

---

# Interfaces

The package follows the Interface Segregation Principle by exposing specialized interfaces.

Available interfaces:

- `IFileUploader`
- `IFileDownloader`
- `IFileDeleter`
- `IFileMover`
- `IFileInfoProvider`

For convenience, all interfaces are combined into:

```csharp
IFileStorage
```

allowing applications to inject a single service when all file operations are required.

---

# LocalFileStorage

The default implementation stores files on the local file system.

Features include:

- Upload files
- Download files
- Delete files
- Move files
- Copy files
- Read file metadata
- Automatic directory creation
- Safe path handling

---

# LocalFileStorageOptions

Configure the storage location through `LocalFileStorageOptions`.

```csharp
new LocalFileStorageOptions
{
    RootPath = Path.Combine(
        Directory.GetCurrentDirectory(),
        "wwwroot/uploads")
}
```

All file operations are performed relative to the configured root directory.

---

# FileMetadata

Retrieve detailed information about stored files.

Typical metadata includes:

- File name
- File path
- File size
- Content type
- Creation date
- Last modified date

Example:

```csharp
Outcome<FileMetadata> metadata =
    await _fileStorage.GetInfoAsync(path);
```

---

# Outcome-Based API

All operations return `Outcome` objects.

Example:

```csharp
var result = await _fileStorage.UploadAsync(
    stream,
    "image.jpg");

if (result.IsSuccess)
{
    string path = result.Value;
}
else
{
    Console.WriteLine(result.Error.Message);
}
```

---

# Dependency Injection

Register the local file storage service:

```csharp
services.AddFileStorage(
    new LocalFileStorageOptions
    {
        RootPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot/uploads")
    });
```

After registration, `IFileStorage` can be injected anywhere in your application.

---

# Example

```csharp
using ImanSoftware.FileStorage;

public class FileService
{
    private readonly IFileStorage _fileStorage;

    public FileService(IFileStorage fileStorage)
    {
        _fileStorage = fileStorage;
    }

    public async Task<Outcome<string>> UploadAsync(
        IFormFile file)
    {
        using var stream = file.OpenReadStream();

        return await _fileStorage.UploadAsync(
            stream,
            file.FileName);
    }
}
```

---

# Additional Operations

Download a file:

```csharp
var result = await _fileStorage.DownloadAsync(path);
```

Delete a file:

```csharp
await _fileStorage.DeleteAsync(path);
```

Move a file:

```csharp
await _fileStorage.MoveAsync(source, destination);
```

Copy a file:

```csharp
await _fileStorage.CopyAsync(source, destination);
```

Retrieve metadata:

```csharp
var metadata =
    await _fileStorage.GetInfoAsync(path);
```

---

# Design Goals

- Storage provider independent
- Clean abstraction
- Dependency Injection friendly
- Async-first API
- Lightweight
- Easy to extend
- Mock-friendly
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