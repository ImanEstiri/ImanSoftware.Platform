// Core/Extensions/ObjectExtensions.cs
using System.Text.Json;

namespace ImanSoftware.Framework.Core.Extensions;

public static class ObjectExtensions
{
    /// <summary>
    /// Converts an object to a JSON string.
    /// </summary>
    public static string ToJson(this object obj, bool indented = false)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = indented,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        return JsonSerializer.Serialize(obj, options);
    }

    /// <summary>
    /// Checks if an object is null.
    /// </summary>
    public static bool IsNull(this object? obj)
        => obj is null;

    /// <summary>
    /// Throws an ArgumentNullException if the object is null.
    /// </summary>
    public static T ThrowIfNull<T>(this T? obj, string paramName) where T : class
    {
        if (obj is null)
            throw new ArgumentNullException(paramName);
        return obj;
    }

    /// <summary>
    /// Creates a shallow copy of an object using serialization.
    /// </summary>
    public static T? DeepCopy<T>(this T obj) where T : class
    {
        var json = JsonSerializer.Serialize(obj);
        return JsonSerializer.Deserialize<T>(json);
    }
}