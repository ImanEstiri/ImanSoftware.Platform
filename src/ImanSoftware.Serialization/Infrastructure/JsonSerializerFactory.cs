// ImanSoftware.Serialization/JsonSerializerFactory.cs
using System.Text.Json;
using Newtonsoft.Json;

namespace ImanSoftware.Serialization;

/// <summary>
/// Factory for creating IJsonSerializer instances with custom options.
/// </summary>
public static class JsonSerializerFactory
{
    /// <summary>
    /// Creates a System.Text.Json serializer with custom options.
    /// </summary>
    public static IImanJsonSerializer CreateSystemTextJsonSerializer(JsonSerializerOptions? options = null)
    {
        return new SystemTextJsonSerializer(options);
    }

    /// <summary>
    /// Creates a Newtonsoft.Json serializer with custom settings.
    /// </summary>
    public static IImanJsonSerializer CreateNewtonsoftJsonSerializer(JsonSerializerSettings? settings = null)
    {
        return new NewtonsoftJsonSerializer(settings);
    }

    /// <summary>
    /// Creates a System.Text.Json serializer with default options (CamelCase, not indented).
    /// </summary>
    public static IImanJsonSerializer CreateDefaultSystemTextJsonSerializer()
    {
        return new SystemTextJsonSerializer();
    }

    /// <summary>
    /// Creates a Newtonsoft.Json serializer with default settings (CamelCase, ignore nulls).
    /// </summary>
    public static IImanJsonSerializer CreateDefaultNewtonsoftJsonSerializer()
    {
        return new NewtonsoftJsonSerializer();
    }
}