// مسیر: Abstractions/FileStorage/FileStorageRegistration.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ImanSoftware.FileStorage;

public static class FileStorageRegistration
{
    public static IServiceCollection AddFileStorage(
        this IServiceCollection services,
        Action<LocalFileStorageOptions> configureOptions,
        Action<IServiceCollection>? customRegistration = null)
    {
        ArgumentNullException.ThrowIfNull(configureOptions);

        services.Configure(configureOptions);

        if (customRegistration is not null)
        {
            customRegistration(services);
        }
        else
        {
            // ثبت اینترفیس اصلی
            services.TryAddScoped<IFileStorage, LocalFileStorage>();

            // ثبت اینترفیس‌های جزئی (همگی به یک نمونه اشاره می‌کنند)
            services.TryAddScoped<IFileUploader>(sp => sp.GetRequiredService<IFileStorage>());
            services.TryAddScoped<IFileDownloader>(sp => sp.GetRequiredService<IFileStorage>());
            services.TryAddScoped<IFileDeleter>(sp => sp.GetRequiredService<IFileStorage>());
            services.TryAddScoped<IFileInfoProvider>(sp => sp.GetRequiredService<IFileStorage>());
            services.TryAddScoped<IFileMover>(sp => sp.GetRequiredService<IFileStorage>());
        }

        return services;
    }
}