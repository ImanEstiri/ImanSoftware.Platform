using ImanSoftware.Framework.Abstractions.Caching;
using ImanSoftware.Framework.Abstractions.DateTimeProvider;
using ImanSoftware.Framework.Abstractions.GuidGenerator;
using ImanSoftware.Framework.Abstractions.Serialization;
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
}
