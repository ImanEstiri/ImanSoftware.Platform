using ImanSoftware.Framework.Abstractions.Caching;
using ImanSoftware.Framework.Abstractions.DataAccess;
using ImanSoftware.Framework.Abstractions.DateTimeProvider;
using ImanSoftware.Framework.Abstractions.GuidGenerator;
using ImanSoftware.Framework.Abstractions.Jwt;
using ImanSoftware.Framework.Abstractions.Serialization;
using ImanSoftware.Framework.Infrastructure.FileStorage;
using ImanSoftware.Framework.Infrastructure.Jwt;
using Microsoft.Extensions.DependencyInjection;

namespace ImanSoftware.Framework.FrameworkDependencyInjection;

public sealed class ImanFrameworkDependencyInjectionOptions
{
    #region DateTimeProvider
    public bool AddDateTimeProvider { get; set; } = true;
    public ImanFrameworkDependencyInjectionCustomImplementation<IDateTimeProvider>? DateTimeProviderCustomImplementation {  get; set; }
    #endregion


    #region GuidGenerator
    public bool AddGuidGenerator { get; set; } = true;
    public GuidGeneratorDefaultImplementationType GuidGeneratorImplementationType { get; set; } = GuidGeneratorDefaultImplementationType.Sequential;
    public ImanFrameworkDependencyInjectionCustomImplementation<IGuidGenerator>? GuidGeneratorCustomImplementation { get; set; }
    #endregion


    #region CurrentUserAccessor
    public bool AddCurrentUserAccessor { get; set; } = true;
    public Action<IServiceCollection>? CurrentUserAccessorCustomRegistration { get; set; }
    #endregion


    #region JsonSerializer
    public bool AddJsonSerializer { get; set; } = true;
    public SerializationDefaultImplementationType SerializationImplementationType { get; set; } = SerializationDefaultImplementationType.Newtonsoft;
    public ImanFrameworkDependencyInjectionCustomImplementation<IJsonSerializer>? JsonSerializerCustomImplementation { get; set; }
    #endregion


    #region CacheService
    public bool AddCacheService { get; set; } = false;
    public ImanFrameworkDependencyInjectionCustomImplementation<ICacheService>? CacheServiceCustomImplementation { get; set; }
    #endregion


    #region Jwt
    public bool AddJwtService { get; set; } = false; 
    public JwtOptions? JwtConfigureOptions { get; set; }
    public ImanFrameworkDependencyInjectionCustomImplementation<IJwtTokenService>? JwtCustomImplementation { get; set; }
    #endregion


    #region DataAccess
    public bool AddDataAccess { get; set; } 
    public string? ConnectionString { get; set; }
    public Action<IServiceCollection>? DataAccessCustomRegistration { get; set; }
    #endregion


    #region FileStorage
    public bool AddFileStorage { get; set; } 
    public Action<LocalFileStorageOptions>? FileStorageConfigureOptions { get; set; }
    public Action<IServiceCollection>? FileStorageCustomRegistration { get; set; }
    #endregion
}
