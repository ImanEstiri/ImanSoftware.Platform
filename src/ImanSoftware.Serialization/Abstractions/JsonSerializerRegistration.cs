using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ImanSoftware.Serialization;

public static class JsonSerializerRegistration
{
    public static IServiceCollection AddJsonSerializer(
        this IServiceCollection services,
        SerializationDefaultImplementationType ImplementationType,
        Type? customImplementation = null)
    {
        if (customImplementation is not null)
        {
            services.AddSingleton(typeof(IJsonSerializer), customImplementation);
        }
        else
        {
            switch (ImplementationType)
            {
                case SerializationDefaultImplementationType.Newtonsoft:
                    services.TryAddSingleton<IJsonSerializer, NewtonsoftJsonSerializer>();
                    break;

                case SerializationDefaultImplementationType.SystemText:
                    services.TryAddSingleton<IJsonSerializer, SystemTextJsonSerializer>();
                    break;

                default:
                    services.TryAddSingleton<IJsonSerializer, NewtonsoftJsonSerializer>();
                    break;
            }
        }

        return services;
    }
}