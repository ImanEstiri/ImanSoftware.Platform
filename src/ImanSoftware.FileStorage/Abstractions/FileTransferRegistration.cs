using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ImanSoftware.FileStorage.Infrastructure;

namespace ImanSoftware.FileStorage;

public static class FileTransferRegistration
{
    /// <summary>
    /// Registers upload/download operations with the specified storage backend.
    /// Requires configuration options (RootPath, MaxFileSize, ...).
    /// </summary>
    public static IServiceCollection AddImanFileTransfer(
        this IServiceCollection services,
        Action<FileTransferOptions> configureOptions,
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
            services.TryAddScoped<IFileTransfer, LocalFileTransfer>();
            services.TryAddScoped<IFileUploader>(sp => sp.GetRequiredService<IFileTransfer>());
            services.TryAddScoped<IFileDownloader>(sp => sp.GetRequiredService<IFileTransfer>());
        }

        return services;
    }
}