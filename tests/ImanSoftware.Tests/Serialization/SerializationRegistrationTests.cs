using Microsoft.Extensions.DependencyInjection;
using ImanSoftware.Serialization;

namespace ImanSoftware.Tests.Serialization;

public class SerializationRegistrationTests
{
    [Fact]
    public void AddImanSerialization_WithNewtonsoft_ResolvesNewtonsoftSerializer()
    {
        var services = new ServiceCollection();

        services.AddImanSerialization(SerializationDefaultImplementationType.Newtonsoft);

        var serializer = services.BuildServiceProvider().GetRequiredService<IImanJsonSerializer>();
        Assert.IsType<NewtonsoftJsonSerializer>(serializer);
    }

    [Fact]
    public void AddImanSerialization_WithSystemText_ResolvesSystemTextSerializer()
    {
        var services = new ServiceCollection();

        services.AddImanSerialization(SerializationDefaultImplementationType.SystemText);

        var serializer = services.BuildServiceProvider().GetRequiredService<IImanJsonSerializer>();
        Assert.Equal("SystemTextJsonSerializer", serializer.GetType().Name);
    }

    [Fact]
    public void AddImanSerialization_RegistersSingleton()
    {
        var services = new ServiceCollection();

        services.AddImanSerialization(SerializationDefaultImplementationType.Newtonsoft);

        var descriptor = Assert.Single(services, d => d.ServiceType == typeof(IImanJsonSerializer));
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
    }

    [Fact]
    public void AddImanSerialization_TryAdd_PreservesExistingRegistration()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IImanJsonSerializer, NewtonsoftJsonSerializer>();

        services.AddImanSerialization(SerializationDefaultImplementationType.SystemText);

        var descriptor = Assert.Single(services, d => d.ServiceType == typeof(IImanJsonSerializer));
        Assert.Equal(typeof(NewtonsoftJsonSerializer), descriptor.ImplementationType);
    }

    [Fact]
    public void AddImanSerialization_WithCustomImplementation_RegistersCustomType()
    {
        var services = new ServiceCollection();

        services.AddImanSerialization(
            SerializationDefaultImplementationType.Newtonsoft,
            typeof(CustomJsonSerializer));

        var serializer = services.BuildServiceProvider().GetRequiredService<IImanJsonSerializer>();
        Assert.IsType<CustomJsonSerializer>(serializer);
    }

    [Fact]
    public void AddImanSerialization_ReturnsSameServices()
    {
        var services = new ServiceCollection();

        Assert.Same(services, services.AddImanSerialization(SerializationDefaultImplementationType.Newtonsoft));
    }

    private sealed class CustomJsonSerializer : IImanJsonSerializer
    {
        public string Serialize<T>(T value) => "custom";
        public string Serialize<T>(T value, object? options = null) => "custom";
        public T? Deserialize<T>(string json) => default;
        public T? Deserialize<T>(string json, object? options = null) => default;
    }
}
