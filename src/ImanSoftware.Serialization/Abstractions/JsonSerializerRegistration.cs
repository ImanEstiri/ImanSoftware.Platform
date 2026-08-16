using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ImanSoftware.Serialization;

public static class ImanJsonSerializerRegistration
{
    public static IServiceCollection AddImanSerialization(
        this IServiceCollection services,
        SerializationDefaultImplementationType ImplementationType,
        Type? customImplementation = null)
    {
        if (customImplementation is not null)
        {
            services.AddSingleton(typeof(IImanJsonSerializer), customImplementation);
        }
        else
        {
            switch (ImplementationType)
            {
                case SerializationDefaultImplementationType.Newtonsoft:
                    services.TryAddSingleton<IImanJsonSerializer, NewtonsoftJsonSerializer>();
                    break;

                case SerializationDefaultImplementationType.SystemText:
                    services.TryAddSingleton<IImanJsonSerializer, SystemTextJsonSerializer>();
                    break;

                default:
                    services.TryAddSingleton<IImanJsonSerializer, NewtonsoftJsonSerializer>();
                    break;
            }
        }

        return services;
    }
}