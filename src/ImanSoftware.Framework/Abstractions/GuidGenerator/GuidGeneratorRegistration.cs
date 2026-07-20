using ImanSoftware.Framework.Infrastructure.GuidGenerator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ImanSoftware.Framework.Abstractions.GuidGenerator;

internal static class GuidGeneratorRegistration
{
    public static IServiceCollection AddGuidGenerator(
    this IServiceCollection services,
    GuidGeneratorDefaultImplementationType ImplementationType = GuidGeneratorDefaultImplementationType.Sequential,
    Type? customImplementation = null)
    {
        if (customImplementation is not null)
            services.AddSingleton(typeof(IGuidGenerator), customImplementation);
        else
        {
            switch (ImplementationType)
            {
                case GuidGeneratorDefaultImplementationType.Random:
                    services.TryAddSingleton<IGuidGenerator, RandomGuidGenerator>();
                    break;
                case GuidGeneratorDefaultImplementationType.Sequential:
                    services.TryAddSingleton<IGuidGenerator, SequentialGuidGenerator>();
                    break;
                default:
                    services.TryAddSingleton<IGuidGenerator, SequentialGuidGenerator>();
                    break;
            }

        }

        return services;
    }
}
