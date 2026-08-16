using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ImanSoftware.Serialization;

/// <summary>
/// Implementation of IJsonSerializer using Newtonsoft.Json.
/// </summary>
public class NewtonsoftJsonSerializer : IImanJsonSerializer
{
    private readonly JsonSerializerSettings _defaultSettings;

    public NewtonsoftJsonSerializer(JsonSerializerSettings? settings = null)
    {
        _defaultSettings = settings ?? new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.None
        };
    }

    public string Serialize<T>(T value)
        => Serialize(value, null);

    public string Serialize<T>(T value, object? options = null)
    {
        var settings = options as JsonSerializerSettings ?? _defaultSettings;
        return JsonConvert.SerializeObject(value, settings);
    }

    public T? Deserialize<T>(string json)
        => Deserialize<T>(json, null);

    public T? Deserialize<T>(string json, object? options = null)
    {
        var settings = options as JsonSerializerSettings ?? _defaultSettings;
        return JsonConvert.DeserializeObject<T>(json, settings);
    }
}