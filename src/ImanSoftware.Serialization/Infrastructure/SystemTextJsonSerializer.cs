using System.Text.Json;

namespace ImanSoftware.Serialization;


/// <summary>
/// Default implementation of IJsonSerializer using System.Text.Json.
/// </summary>
internal class SystemTextJsonSerializer : IJsonSerializer
{
    private readonly JsonSerializerOptions _defaultOptions;

    public SystemTextJsonSerializer(JsonSerializerOptions? options = null)
    {
        _defaultOptions = options ?? new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    public string Serialize<T>(T value)
    {
        return JsonSerializer.Serialize(value, _defaultOptions);
    }

    public T? Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, _defaultOptions);
    }

    public T? Deserialize<T>(string json, object? options = null)
    {
        var serializerOptions = options as JsonSerializerOptions ?? _defaultOptions;
        return JsonSerializer.Deserialize<T>(json, serializerOptions);
    }
}