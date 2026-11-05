using ImanSoftware.Serialization;

namespace ImanSoftware.Tests.Serialization;

public class JsonSerializerFactoryTests
{
    [Fact]
    public void CreateSystemTextJsonSerializer_ReturnsSystemTextImplementation()
    {
        var serializer = JsonSerializerFactory.CreateSystemTextJsonSerializer();

        Assert.IsAssignableFrom<IImanJsonSerializer>(serializer);
        Assert.Equal("SystemTextJsonSerializer", serializer.GetType().Name);
    }

    [Fact]
    public void CreateNewtonsoftJsonSerializer_ReturnsNewtonsoftImplementation()
    {
        var serializer = JsonSerializerFactory.CreateNewtonsoftJsonSerializer();

        Assert.IsType<NewtonsoftJsonSerializer>(serializer);
    }

    [Fact]
    public void CreateDefaultSystemTextJsonSerializer_ReturnsFreshInstanceEachCall()
    {
        var first = JsonSerializerFactory.CreateDefaultSystemTextJsonSerializer();
        var second = JsonSerializerFactory.CreateDefaultSystemTextJsonSerializer();

        Assert.NotSame(first, second);
    }

    [Fact]
    public void CreateDefaultNewtonsoftJsonSerializer_ReturnsFreshInstanceEachCall()
    {
        var first = JsonSerializerFactory.CreateDefaultNewtonsoftJsonSerializer();
        var second = JsonSerializerFactory.CreateDefaultNewtonsoftJsonSerializer();

        Assert.NotSame(first, second);
    }

    [Fact]
    public void FactoryProducts_SerializeAndDeserializeConsistently()
    {
        var value = new { Name = "x", Value = 3 };

        foreach (var serializer in new[]
                 {
                     JsonSerializerFactory.CreateDefaultSystemTextJsonSerializer(),
                     JsonSerializerFactory.CreateDefaultNewtonsoftJsonSerializer()
                 })
        {
            var json = serializer.Serialize(value);
            Assert.Contains("Name", json, StringComparison.OrdinalIgnoreCase);

            var anonymous = serializer.Deserialize<Dictionary<string, object>>(json);
            Assert.NotNull(anonymous);
        }
    }
}
