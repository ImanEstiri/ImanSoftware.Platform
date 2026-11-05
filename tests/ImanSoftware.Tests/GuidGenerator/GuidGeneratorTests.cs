using Microsoft.Extensions.DependencyInjection;
using ImanSoftware.Framework.Infrastructure.GuidGenerator;
using ImanSoftware.GuidGenerator;

namespace ImanSoftware.Tests.GuidGenerator;

public class SequentialGuidGeneratorTests
{
    [Fact]
    public void NewGuid_ReturnsNonEmptyGuid()
    {
        Assert.NotEqual(Guid.Empty, new SequentialGuidGenerator().NewGuid());
    }

    [Fact]
    public void NewGuid_ProducesUniqueValues()
    {
        var generator = new SequentialGuidGenerator();
        var generated = new HashSet<Guid>(Enumerable.Range(0, 100).Select(_ => generator.NewGuid()));

        Assert.Equal(100, generated.Count);
    }

    [Fact]
    public void NewGuid_EncodesTimestampInLastSixBytes()
    {
        var beforeTicks = DateTime.Now.Ticks / 10000L;
        var guid = new SequentialGuidGenerator().NewGuid();
        var afterTicks = DateTime.Now.Ticks / 10000L;

        var tail = GuidTailToMilliseconds(guid);

        Assert.InRange(tail, Low48Bits(beforeTicks), Low48Bits(afterTicks));
    }

    [Fact]
    public void NewGuid_TailIncreasesOverTime()
    {
        var generator = new SequentialGuidGenerator();

        var first = GuidTailToMilliseconds(generator.NewGuid());
        Thread.Sleep(50);
        var second = GuidTailToMilliseconds(generator.NewGuid());

        Assert.True(second >= first);
    }

    [Fact]
    public void NewGuid_PreservesRandomPrefix()
    {
        var generator = new SequentialGuidGenerator();

        // First 10 bytes stay random; two guids generated quickly differ in their prefix.
        var guids = Enumerable.Range(0, 20).Select(_ => generator.NewGuid()).ToArray();
        var prefixes = new HashSet<string>(guids.Select(g => string.Concat(g.ToByteArray().Take(10))));

        Assert.True(prefixes.Count > 1);
    }

    private static long GuidTailToMilliseconds(Guid guid)
    {
        var bytes = guid.ToByteArray();
        long tail = 0;
        for (var i = 10; i < 16; i++)
        {
            tail = (tail << 8) | bytes[i];
        }

        return tail;
    }

    private static long Low48Bits(long value) => value & 0xFFFFFFFFFFFF;
}

public class RandomGuidGeneratorTests
{
    [Fact]
    public void NewGuid_ReturnsNonEmptyGuid()
    {
        Assert.NotEqual(Guid.Empty, new RandomGuidGenerator().NewGuid());
    }

    [Fact]
    public void NewGuid_ProducesUniqueValues()
    {
        var generator = new RandomGuidGenerator();
        var generated = new HashSet<Guid>(Enumerable.Range(0, 100).Select(_ => generator.NewGuid()));

        Assert.Equal(100, generated.Count);
    }
}

public class GuidGeneratorRegistrationTests
{
    [Fact]
    public void AddGuidGenerator_DefaultSequential_RegistersSequentialImplementation()
    {
        var services = new ServiceCollection();

        services.AddGuidGenerator();

        var descriptor = Assert.Single(services, d => d.ServiceType == typeof(IGuidGenerator));
        Assert.Equal(typeof(SequentialGuidGenerator), descriptor.ImplementationType);
    }

    [Fact]
    public void AddGuidGenerator_Random_RegistersRandomImplementation()
    {
        var services = new ServiceCollection();

        services.AddGuidGenerator(GuidGeneratorDefaultImplementationType.Random);

        var descriptor = Assert.Single(services, d => d.ServiceType == typeof(IGuidGenerator));
        Assert.Equal(typeof(RandomGuidGenerator), descriptor.ImplementationType);
    }

    [Fact]
    public void AddGuidGenerator_RegistersSingleton()
    {
        var services = new ServiceCollection();

        services.AddGuidGenerator(GuidGeneratorDefaultImplementationType.Sequential);

        Assert.All(
            services.Where(d => d.ServiceType == typeof(IGuidGenerator)),
            d => Assert.Equal(ServiceLifetime.Singleton, d.Lifetime));
    }

    [Fact]
    public void AddGuidGenerator_TryAdd_PreservesExistingRegistration()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IGuidGenerator, FixedGuidGenerator>();

        services.AddGuidGenerator(GuidGeneratorDefaultImplementationType.Random);

        var descriptor = Assert.Single(services, d => d.ServiceType == typeof(IGuidGenerator));
        Assert.Equal(typeof(FixedGuidGenerator), descriptor.ImplementationType);
    }

    [Fact]
    public void AddGuidGenerator_WithCustomImplementation_UsesAddSemantics()
    {
        var services = new ServiceCollection();

        services.AddGuidGenerator(GuidGeneratorDefaultImplementationType.Sequential, typeof(FixedGuidGenerator));

        var descriptor = Assert.Single(services, d => d.ServiceType == typeof(IGuidGenerator));
        Assert.Equal(typeof(FixedGuidGenerator), descriptor.ImplementationType);
    }

    private sealed class FixedGuidGenerator : IGuidGenerator
    {
        public Guid NewGuid() => Guid.Empty;
    }
}
