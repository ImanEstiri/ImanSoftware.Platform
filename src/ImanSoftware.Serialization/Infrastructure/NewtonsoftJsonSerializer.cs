using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ImanSoftware.Serialization;

/// <summary>
/// Implementation of IJsonSerializer using Newtonsoft.Json.
/// </summary>
public class NewtonsoftJsonSerializer : IJsonSerializer
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
    {
        return JsonConvert.SerializeObject(value, _defaultSettings);
    }

    public T? Deserialize<T>(string json)
    {
        return JsonConvert.DeserializeObject<T>(json, _defaultSettings);
    }

    public T? Deserialize<T>(string json, object? options = null)
    {
        var settings = options as JsonSerializerSettings ?? _defaultSettings;
        return JsonConvert.DeserializeObject<T>(json, settings);
    }
}