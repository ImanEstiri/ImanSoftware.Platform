using Newtonsoft.Json;
using ImanSoftware.Serialization;

namespace ImanSoftware.Tests.Serialization;

public class NewtonsoftJsonSerializerTests
{
    private sealed class SampleDto
    {
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
        public string? Optional { get; set; }
        public string? LongPropertyName { get; set; }
    }

    [Fact]
    public void Serialize_UsesCamelCaseByDefault()
    {
        var serializer = new NewtonsoftJsonSerializer();

        var json = serializer.Serialize(new SampleDto { Name = "x", LongPropertyName = "y" });

        Assert.Contains("\"longPropertyName\":\"y\"", json, StringComparison.Ordinal);
    }

    [Fact]
    public void Serialize_IgnoresNullValuesByDefault()
    {
        var serializer = new NewtonsoftJsonSerializer();

        var json = serializer.Serialize(new SampleDto { Name = "x" });

        Assert.DoesNotContain("optional", json, StringComparison.Ordinal);
    }

    [Fact]
    public void Deserialize_IsCaseInsensitiveByDefault()
    {
        var serializer = new NewtonsoftJsonSerializer();

        var dto = serializer.Deserialize<SampleDto>("""{"Name":"x","VALUE":5}""");

        Assert.NotNull(dto);
        Assert.Equal("x", dto!.Name);
        Assert.Equal(5, dto.Value);
    }

    [Fact]
    public void Serialize_WithCustomSettings_UsesProvidedSettings()
    {
        var serializer = new NewtonsoftJsonSerializer();
        var settings = new JsonSerializerSettings { ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver() };

        var json = serializer.Serialize(new SampleDto { LongPropertyName = "y" }, settings);

        Assert.Contains("\"LongPropertyName\":\"y\"", json, StringComparison.Ordinal);
    }

    [Fact]
    public void Serialize_WithWrongOptionsType_FallsBackToDefaults()
    {
        var serializer = new NewtonsoftJsonSerializer();

        var json = serializer.Serialize(new SampleDto { Name = "x" }, options: 12345);

        Assert.Contains("\"name\":\"x\"", json, StringComparison.Ordinal);
    }

    [Fact]
    public void Deserialize_WithNullJson_ReturnsDefault()
    {
        var serializer = new NewtonsoftJsonSerializer();

        Assert.Null(serializer.Deserialize<SampleDto>("null"));
    }

    [Fact]
    public void Ctor_WithExplicitSettings_UsesThoseSettings()
    {
        var serializer = new NewtonsoftJsonSerializer(new JsonSerializerSettings
        {
            ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver(),
            NullValueHandling = NullValueHandling.Include
        });

        var json = serializer.Serialize(new SampleDto { Name = "x" });

        Assert.Contains("\"Optional\":null", json, StringComparison.Ordinal);
    }
}
