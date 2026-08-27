# ImanSoftware.Branding

**Version:** 1.0.0 | **Last Updated:** August 2026

> Display ImanSoftware branding badge with contact information in Blazor apps.

`ImanSoftware.Branding` is a lightweight, plug-and-play Blazor component that displays a customizable badge with developer branding and contact information. It fetches data from a remote API (with a fallback backup) and caches it using `ImanSoftware.Caching` for optimal performance.

---

## Installation

Install the package using the .NET CLI:

```bash
dotnet add package ImanSoftware.Branding
```

---

## Why ImanSoftware.Branding?

Adding developer or company branding to a Blazor app usually means hand-rolling a badge, a modal, an API call, and caching logic every single time.

This package provides all of that out of the box, so you can add a consistent, professional branding badge to any project with a single line of code.

Benefits include:

- One-line setup
- Smart caching for optimal performance
- Primary + backup API fallback
- Graceful degradation to a default/custom text
- Built-in logging for diagnostics
- Clean, modern modal for contact info
- Fully customizable styles, texts, and endpoints
- Works with Blazor Server and Blazor WebAssembly

---

# Features

- ✅ **One-line setup** – just call `AddImanBranding()` in your `Program.cs`.
- ✅ **Blazor compatible** – works with Blazor Server and Blazor WebAssembly.
- ✅ **Smart caching** – uses `ImanSoftware.Caching` (cache duration configurable).
- ✅ **API fallback** – Primary API (with `projectId`) + Backup API (without `projectId`).
- ✅ **Graceful degradation** – if both APIs fail, falls back to a default/custom text.
- ✅ **Built-in logging** – uses `ImanSoftware.Logging` for diagnostic messages.
- ✅ **Clean, modern modal** – displays developer contact info on click.
- ✅ **Fully customizable** – override styles, texts, and API endpoints.

---

# Quick Start

### 1. Register the service in `Program.cs`

```csharp
using ImanSoftware.Branding.DependencyInjection;

builder.Services.AddImanBranding();
```

You can optionally customize the behavior:

```csharp
builder.Services.AddImanBranding(options =>
{
    options.PrimaryApiUrl = "https://your-api.com/badge";
    options.BackupApiUrl = "https://your-backup.com/badge";
    options.DefaultBadgeText = "Built with ❤️ by ImanSoftware";
    options.CacheDuration = TimeSpan.FromHours(6);
    options.RequestTimeoutSeconds = 2;
});
```

### 2. Use the component in any `.razor` file

```razor
@using ImanSoftware.Branding.Components

<footer>
    <ImanSoftwareBadge ProjectId="my-project-id" />
</footer>
```

You can also override the fallback text:

```razor
<ImanSoftwareBadge ProjectId="my-project" CustomBadgeText="Custom Fallback Text" />
```

---

# Configuration

| Option | Description | Default |
| :--- | :--- | :--- |
| `PrimaryApiUrl` | Main API endpoint (receives `projectId`) | `https://imansoftware.ir/api/badge` |
| `BackupApiUrl` | Fallback endpoint (no `projectId` required) | `""` (disabled by default) |
| `DefaultBadgeText` | Text shown when both APIs fail | `"Designed & Developed by ImanSoftware"` |
| `CacheDuration` | How long to cache the badge data | `12 hours` |
| `RequestTimeoutSeconds` | Timeout for each HTTP request | `3 seconds` |

---

# API Response Format

Both APIs should return a JSON object matching this structure:

```json
{
  "developer": "Iman Estiri",
  "brand": "ImanSoftware",
  "website": "https://imansoftware.ir",
  "phone": "+98 912 XXX XXXX",
  "telegram": "@imanestiri",
  "email": "info@imansoftware.ir",
  "linkedin": "https://linkedin.com/in/imanestiri",
  "badgeText": "Designed & Developed by ImanSoftware"
}
```

> The `badgeText` field is displayed on the badge. All other fields appear in the modal.

---

# Styling

The badge and modal come with built-in, minimal styles. You can override them in your own CSS:

```css
.imansoftware-badge { /* your styles */ }
.modal-overlay { /* your styles */ }
.modal-content { /* your styles */ }
```

---

# How It Works

1. **On component initialization**:
   - Tries to get cached data for the given `ProjectId`.
   - If cached data exists → displays it immediately.
   - If not → calls `IBrandingService.GetBadgeDataAsync()`.
2. **BrandingService**:
   - First attempts the **Primary API** with `?projectId=...`.
   - If that fails, it tries the **Backup API** (if configured) without any query parameter.
   - If both fail → returns `null`.
3. **Caching**:
   - On success, the result is cached using `ImanSoftware.Caching` for the duration specified in `CacheDuration`.
4. **Display**:
   - If data is available, the badge text from `badgeText` is shown.
   - If no data is available, the fallback text (`DefaultBadgeText` or `CustomBadgeText`) is used.
5. **Modal**:
   - Clicking the badge opens a modal showing all contact information.

---

# Dependencies

| Package | Version |
| :--- | :--- |
| `ImanSoftware.Caching` | latest |
| `ImanSoftware.Logging` | latest |
| `Microsoft.Extensions.Http` | 8.0+ |
| `Microsoft.AspNetCore.Components` | 8.0+ |

---

# Complete Example

```razor
@page "/"
@using ImanSoftware.Branding.Components

<PageTitle>My Awesome App</PageTitle>

<h1>Welcome to My App</h1>

<footer style="margin-top: 50px; text-align: center;">
    <ImanSoftwareBadge ProjectId="awesome-app" />
</footer>
```

---

# Notes

- The `ProjectId` is **required** – it helps the Primary API return custom content per project.
- If you don't specify a `BackupApiUrl`, the fallback is completely skipped (no extra HTTP call).
- The package works in both **Blazor Server** and **Blazor WebAssembly** out of the box.

---

# Requirements

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
