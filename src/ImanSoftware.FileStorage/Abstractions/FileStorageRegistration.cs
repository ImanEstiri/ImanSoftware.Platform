using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ImanSoftware.FileStorage.Infrastructure;

namespace ImanSoftware.FileStorage;

public static class FileStorageRegistration
{
    /// <summary>
    /// Registers general-purpose file storage operations.
    /// No configuration needed - works with any file path.
    /// </summary>
    public static IServiceCollection AddImanFileStorage(
        this IServiceCollection services,
        Action<IServiceCollection>? customRegistration = null)
    {
        if (customRegistration is not null)
        {
            customRegistration(services);
        }
        else
        {
            services.TryAddScoped<IFileStorage, LocalFileStorage>();

            services.TryAddScoped<IFileReader>(sp => sp.GetRequiredService<IFileStorage>());
            services.TryAddScoped<IFileWriter>(sp => sp.GetRequiredService<IFileStorage>());
            services.TryAddScoped<IFileDeleter>(sp => sp.GetRequiredService<IFileStorage>());
            services.TryAddScoped<IFileMover>(sp => sp.GetRequiredService<IFileStorage>());
            services.TryAddScoped<IFileInfoProvider>(sp => sp.GetRequiredService<IFileStorage>());
            services.TryAddScoped<IDirectoryOperator>(sp => sp.GetRequiredService<IFileStorage>());
        }

        return services;
    }
}