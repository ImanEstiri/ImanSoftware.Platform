using System.Text.Json;
using ImanSoftware.Serialization;

namespace ImanSoftware.Tests.Serialization;

public class SystemTextJsonSerializerTests
{
    private sealed class SampleDto
    {
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
        public string? Optional { get; set; }
        public string? LongPropertyName { get; set; }
    }

    private static IImanJsonSerializer CreateSerializer() =>
        JsonSerializerFactory.CreateDefaultSystemTextJsonSerializer();

    [Fact]
    public void Serialize_UsesCamelCaseAndSkipsIndentation()
    {
        var serializer = CreateSerializer();

        var json = serializer.Serialize(new SampleDto { Name = "x", Value = 5 });

        Assert.Equal("""{"name":"x","value":5,"optional":null,"longPropertyName":null}""", json);
    }

    [Fact]
    public void Deserialize_WithCamelCaseJson_RoundTrips()
    {
        var serializer = CreateSerializer();

        var dto = serializer.Deserialize<SampleDto>("""{"name":"x","value":5}""");

        Assert.NotNull(dto);
        Assert.Equal("x", dto!.Name);
        Assert.Equal(5, dto.Value);
    }

    [Fact]
    public void Deserialize_IsCaseSensitiveByDefault()
    {
        var serializer = CreateSerializer();

        var dto = serializer.Deserialize<SampleDto>("""{"Name":"x"}""");

        Assert.NotNull(dto);
        Assert.Equal(string.Empty, dto!.Name);
    }

    [Fact]
    public void Serialize_WithCustomOptions_UsesProvidedOptions()
    {
        var serializer = CreateSerializer();
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };

        var json = serializer.Serialize(new SampleDto { LongPropertyName = "x" }, options);

        Assert.Contains("\"long_property_name\":\"x\"", json, StringComparison.Ordinal);
    }

    [Fact]
    public void Serialize_WithWrongOptionsType_FallsBackToDefaults()
    {
        var serializer = CreateSerializer();

        var json = serializer.Serialize(new SampleDto { Name = "x" }, options: "not json options");

        Assert.Contains("\"longPropertyName\":null", json, StringComparison.Ordinal);
    }

    [Fact]
    public void Deserialize_WithNullJson_ReturnsDefault()
    {
        var serializer = CreateSerializer();

        Assert.Null(serializer.Deserialize<SampleDto>("null"));
    }

    [Fact]
    public void Ctor_WithIndentedOptions_UsesThoseOptions()
    {
        var serializer = JsonSerializerFactory.CreateSystemTextJsonSerializer(
            new JsonSerializerOptions { WriteIndented = true });

        var json = serializer.Serialize(new SampleDto { Name = "x" });

        Assert.Contains(Environment.NewLine, json, StringComparison.Ordinal);
    }
}
