using System.Text.Json;

namespace ImanSoftware.Serialization;


/// <summary>
/// Default implementation of IJsonSerializer using System.Text.Json.
/// </summary>
internal class SystemTextJsonSerializer : IImanJsonSerializer
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
        => Serialize(value, null);

    public string Serialize<T>(T value, object? options = null)
    {
        var serializerOptions = options as JsonSerializerOptions ?? _defaultOptions;
        return JsonSerializer.Serialize(value, serializerOptions);
    }

    public T? Deserialize<T>(string json)
        => Deserialize<T>(json, null);

    public T? Deserialize<T>(string json, object? options = null)
    {
        var serializerOptions = options as JsonSerializerOptions ?? _defaultOptions;
        return JsonSerializer.Deserialize<T>(json, serializerOptions);
    }
}