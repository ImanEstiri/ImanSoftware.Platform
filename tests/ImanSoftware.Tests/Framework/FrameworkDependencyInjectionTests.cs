using Microsoft.Extensions.DependencyInjection;
using ImanSoftware.Caching;
using ImanSoftware.CQRS;
using ImanSoftware.CQRS.DependencyInjection;
using ImanSoftware.CurrentUserAccessor;
using ImanSoftware.DataAccess;
using ImanSoftware.DataAccess.Dapper;
using ImanSoftware.DateTimeProvider;
using ImanSoftware.FileStorage;
using ImanSoftware.FileStorage.Infrastructure;
using ImanSoftware.Framework.FrameworkDependencyInjection;
using ImanSoftware.Framework.Infrastructure.GuidGenerator;
using ImanSoftware.GuidGenerator;
using ImanSoftware.Tests.CQRS;
using ImanSoftware.Jwt;
using ImanSoftware.Serialization;

namespace ImanSoftware.Tests.Framework;

public class FrameworkDependencyInjectionTests
{
    private static ImanFrameworkDependencyInjectionOptions CreateOptions(
        Action<ImanFrameworkDependencyInjectionOptions>? configure = null)
    {
        var options = new ImanFrameworkDependencyInjectionOptions();
        configure?.Invoke(options);
        return options;
    }

    [Fact]
    public void AddImanFramework_WithDefaults_RegistersDefaultCoreServices()
    {
        var services = new ServiceCollection();

        services.AddImanFramework(CreateOptions());

        using var provider = services.BuildServiceProvider();
        Assert.IsType<DefaultDateTimeProvider>(provider.GetRequiredService<IDateTimeProvider>());
        Assert.IsType<SequentialGuidGenerator>(provider.GetRequiredService<IGuidGenerator>());
        Assert.IsType<HttpContextUserAccessor<int>>(provider.GetRequiredService<ICurrentUserAccessor<int>>());
        Assert.IsType<NewtonsoftJsonSerializer>(provider.GetRequiredService<IImanJsonSerializer>());
    }

    [Fact]
    public void AddImanFramework_WithDefaults_DoesNotRegisterOptionalServices()
    {
        var services = new ServiceCollection();

        services.AddImanFramework(CreateOptions());

        Assert.DoesNotContain(services, d => d.ServiceType == typeof(ICacheService));
        Assert.DoesNotContain(services, d => d.ServiceType == typeof(IJwtTokenService));
        Assert.DoesNotContain(services, d => d.ServiceType == typeof(IDbConnectionFactory));
        Assert.DoesNotContain(services, d => d.ServiceType == typeof(IFileStorage));
    }

    [Fact]
    public void AddImanFramework_WithNullOptions_UsesDefaults()
    {
        var services = new ServiceCollection();

        services.AddImanFramework(null!);

        using var provider = services.BuildServiceProvider();
        Assert.IsType<DefaultDateTimeProvider>(provider.GetRequiredService<IDateTimeProvider>());
    }

    [Fact]
    public void AddImanFramework_WithCacheServiceEnabled_RegistersCacheAndKeepsFirstSerializer()
    {
        var services = new ServiceCollection();

        services.AddImanFramework(CreateOptions(o => o.AddCacheService = true));

        using var provider = services.BuildServiceProvider();
        Assert.IsType<MemoryCacheService>(provider.GetRequiredService<ICacheService>());

        // JsonSerializer was registered first (Newtonsoft default); AddCacheService's
        // SystemText registration uses TryAdd and must not override it.
        Assert.IsType<NewtonsoftJsonSerializer>(provider.GetRequiredService<IImanJsonSerializer>());
    }

    [Fact]
    public void AddImanFramework_WithRandomGuidGenerator_RegistersRandomImplementation()
    {
        var services = new ServiceCollection();

        services.AddImanFramework(CreateOptions(o =>
        {
            o.AddGuidGenerator = true;
            o.GuidGeneratorImplementationType = GuidGeneratorDefaultImplementationType.Random;
        }));

        using var provider = services.BuildServiceProvider();
        Assert.IsType<RandomGuidGenerator>(provider.GetRequiredService<IGuidGenerator>());
    }

    [Fact]
    public void AddImanFramework_WithSystemTextSerializer_RegistersSystemTextImplementation()
    {
        var services = new ServiceCollection();

        services.AddImanFramework(CreateOptions(o =>
        {
            o.SerializationImplementationType = SerializationDefaultImplementationType.SystemText;
        }));

        using var provider = services.BuildServiceProvider();
        Assert.Equal("SystemTextJsonSerializer", provider.GetRequiredService<IImanJsonSerializer>().GetType().Name);
    }

    [Fact]
    public void AddImanFramework_WithCustomImplementations_UsesThem()
    {
        var services = new ServiceCollection();

        services.AddImanFramework(CreateOptions(o =>
        {
            o.DateTimeProviderCustomImplementation = typeof(FixedDateTimeProvider);
            o.GuidGeneratorCustomImplementation = typeof(FixedGuidGenerator);
            o.JsonSerializerCustomImplementation = typeof(CustomJsonSerializer);
        }));

        using var provider = services.BuildServiceProvider();
        Assert.IsType<FixedDateTimeProvider>(provider.GetRequiredService<IDateTimeProvider>());
        Assert.IsType<FixedGuidGenerator>(provider.GetRequiredService<IGuidGenerator>());
        Assert.IsType<CustomJsonSerializer>(provider.GetRequiredService<IImanJsonSerializer>());
    }

    [Fact]
    public void AddImanFramework_AddJwtServiceWithoutOptionsOrCustom_ThrowsArgumentException()
    {
        var services = new ServiceCollection();

        var exception = Assert.Throws<ArgumentException>(
            () => services.AddImanFramework(CreateOptions(o => o.AddJwtService = true)));

        // .NET appends "(Parameter 'JwtConfigureOptions')" to the message.
        Assert.StartsWith(
            "ImanSoftware.Abstractions.Jwt.IJwtService Needs Options For Registering Default Implementation.",
            exception.Message,
            StringComparison.Ordinal);
        Assert.Equal("JwtConfigureOptions", exception.ParamName);
    }

    [Fact]
    public void AddImanFramework_AddJwtServiceWithOptions_RegistersWorkingService()
    {
        var services = new ServiceCollection();

        services.AddImanFramework(CreateOptions(o =>
        {
            o.AddJwtService = true;
            o.JwtConfigureOptions = new JwtOptions
            {
                SecretKey = "test-secret-key-for-imansoftware-jwt-tests-must-be-at-least-64-bytes-long!!",
                Issuer = "issuer",
                Audience = "audience"
            };
        }));

        using var provider = services.BuildServiceProvider();
        var tokenService = provider.GetRequiredService<IJwtTokenService>();
        var token = tokenService.GenerateToken([new System.Security.Claims.Claim("sub", "1")]);

        Assert.True(tokenService.ValidateToken(token).IsValid);
    }

    [Fact]
    public void AddImanFramework_AddJwtServiceWithCustomImplementation_RegistersCustom()
    {
        var services = new ServiceCollection();

        services.AddImanFramework(CreateOptions(o =>
        {
            o.AddJwtService = true;
            o.JwtCustomImplementation = typeof(CustomJwtService);
        }));

        using var provider = services.BuildServiceProvider();
        Assert.IsType<CustomJwtService>(provider.GetRequiredService<IJwtTokenService>());
    }

    [Fact]
    public void AddImanFramework_AddDataAccessWithoutConnectionString_Throws()
    {
        var services = new ServiceCollection();

        // ThrowIfNullOrWhiteSpace surfaces as ArgumentNullException for null input.
        var exception = Assert.ThrowsAny<ArgumentException>(
            () => services.AddImanFramework(CreateOptions(o => o.AddDataAccess = true)));

        Assert.Equal("DataAccessConnectionString", exception.ParamName);
    }

    [Fact]
    public void AddImanFramework_AddDataAccessWithoutConnectionString_ThrowsEvenWithCustomRegistration()
    {
        var services = new ServiceCollection();

        Assert.ThrowsAny<ArgumentException>(() => services.AddImanFramework(CreateOptions(o =>
        {
            o.AddDataAccess = true;
            o.DataAccessCustomRegistration = _ => { };
        })));
    }

    [Fact]
    public void AddImanFramework_AddDapperDataAccess_RegistersAccessor()
    {
        var services = new ServiceCollection();

        services.AddImanFramework(CreateOptions(o =>
        {
            o.AddDapperDataAccess = true;
            o.DapperConnectionString = "Server=.;Database=Test";
        }));

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        Assert.IsType<DapperDataAccessor>(scope.ServiceProvider.GetRequiredService<ISqlDataAccessor>());
        Assert.Equal("Server=.;Database=Test", provider.GetRequiredService<IDbConnectionFactory>().ConnectionString);
    }

    [Fact]
    public void AddImanFramework_AddFileStorage_RegistersStorage()
    {
        var services = new ServiceCollection();

        services.AddImanFramework(CreateOptions(o => o.AddFileStorage = true));

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        Assert.IsAssignableFrom<IFileStorage>(scope.ServiceProvider.GetRequiredService<IFileStorage>());
    }

    [Fact]
    public void AddImanFramework_AddFileTransferWithoutConfigureOrCustom_ThrowsArgumentException()
    {
        var services = new ServiceCollection();

        var exception = Assert.Throws<ArgumentException>(
            () => services.AddImanFramework(CreateOptions(o => o.AddFileTransfer = true)));

        Assert.Equal(
            "When using the default FileTransfer implementation, FileTransferConfigureOptions must be provided.",
            exception.Message);
    }

    [Fact]
    public void AddImanFramework_AddFileTransferWithConfigure_RegistersTransfer()
    {
        var services = new ServiceCollection();
        var root = Path.Combine(Path.GetTempPath(), "iman-fw-test-" + Guid.NewGuid().ToString("N"));

        services.AddImanFramework(CreateOptions(o =>
        {
            o.AddFileTransfer = true;
            o.FileTransferConfigureOptions = transferOptions => transferOptions.RootPath = root;
        }));

        // LocalFileTransfer takes a raw FileTransferOptions in its constructor and only
        // IOptions<FileTransferOptions> is registered — resolving it via DI throws unless
        // the raw options instance is also registered (package quirk; asserted via descriptor).
        var descriptor = Assert.Single(services, d => d.ServiceType == typeof(IFileTransfer));
        Assert.Equal(typeof(LocalFileTransfer), descriptor.ImplementationType);
        Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [Fact]
    public void AddImanFramework_AddFileTransferWithOnlyCustomRegistration_SilentlyRegistersNothing()
    {
        var services = new ServiceCollection();

        services.AddImanFramework(CreateOptions(o =>
        {
            o.AddFileTransfer = true;
            o.FileTransferCustomRegistration = _ => { };
        }));

        // Validation passes but no registration happens (documented quirk).
        Assert.DoesNotContain(services, d => d.ServiceType == typeof(IFileTransfer));
    }

    [Fact]
    public void AddImanFramework_DisablingFlag_SkipsRegistration()
    {
        var services = new ServiceCollection();

        services.AddImanFramework(CreateOptions(o =>
        {
            o.AddDateTimeProvider = false;
            o.AddGuidGenerator = false;
            o.AddCurrentUserAccessor = false;
            o.AddJsonSerializer = false;
        }));

        Assert.Empty(services);
    }

    [Fact]
    public void AddImanFramework_WithCqrsStillResolvable_RegistersFrameworkWithoutConflicts()
    {
        var services = new ServiceCollection();

        services.AddImanFramework(CreateOptions());
        services.AddLogging();
        services.AddCqrs([typeof(PingCommand).Assembly]);

        using var provider = services.BuildServiceProvider();
        Assert.NotNull(provider.GetRequiredService<MediatR.IMediator>());
    }

    private sealed class FixedDateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => new(2024, 1, 1);
        public DateOnly Today => new(2024, 1, 1);
        public TimeOnly CurrentTimeOfDay => new(0, 0);
    }

    private sealed class FixedGuidGenerator : IGuidGenerator
    {
        public Guid NewGuid() => Guid.Empty;
    }

    private sealed class CustomJsonSerializer : IImanJsonSerializer
    {
        public string Serialize<T>(T value) => "custom";
        public string Serialize<T>(T value, object? options = null) => "custom";
        public T? Deserialize<T>(string json) => default;
        public T? Deserialize<T>(string json, object? options = null) => default;
    }

    private sealed class CustomJwtService : IJwtTokenService
    {
        public string GenerateToken(IEnumerable<System.Security.Claims.Claim> claims) => "custom";
        public JwtTokenValidationResult ValidateToken(string token) => JwtTokenValidationResult.Failure("custom");
        public System.Security.Claims.ClaimsPrincipal? GetPrincipalFromExpiredToken(string token) => null;
    }
}

public class CustomImplementationOptionsTests
{
    [Fact]
    public void Setter_AcceptsCompatibleType()
    {
        var wrapper = new ImanFrameworkDependencyInjectionCustomImplementation<IDateTimeProvider>
        {
            CustomImplementation = typeof(FixedProvider)
        };

        Assert.Equal(typeof(FixedProvider), wrapper.CustomImplementation);
    }

    [Fact]
    public void Setter_IgnoresIncompatibleType_Silently()
    {
        var wrapper = new ImanFrameworkDependencyInjectionCustomImplementation<IDateTimeProvider>
        {
            CustomImplementation = typeof(FixedProvider)
        };

        wrapper.CustomImplementation = typeof(string); // incompatible → ignored

        Assert.Equal(typeof(FixedProvider), wrapper.CustomImplementation);
    }

    [Fact]
    public void Setter_IgnoresNull()
    {
        var wrapper = new ImanFrameworkDependencyInjectionCustomImplementation<IDateTimeProvider>
        {
            CustomImplementation = typeof(FixedProvider)
        };

        wrapper.CustomImplementation = null;

        Assert.Equal(typeof(FixedProvider), wrapper.CustomImplementation);
    }

    [Fact]
    public void ImplicitOperator_FromCompatibleType_WrapsType()
    {
        ImanFrameworkDependencyInjectionCustomImplementation<IGuidGenerator> wrapper = typeof(FixedGuidGen);

        Assert.Equal(typeof(FixedGuidGen), wrapper.CustomImplementation);
    }

    [Fact]
    public void ImplicitOperator_FromIncompatibleType_ProducesNullCustomImplementation()
    {
        ImanFrameworkDependencyInjectionCustomImplementation<IGuidGenerator> wrapper = typeof(string);

        Assert.Null(wrapper.CustomImplementation);
    }

    private sealed class FixedProvider : IDateTimeProvider
    {
        public DateTime Now => new(2024, 1, 1);
        public DateOnly Today => new(2024, 1, 1);
        public TimeOnly CurrentTimeOfDay => new(0, 0);
    }

    private sealed class FixedGuidGen : IGuidGenerator
    {
        public Guid NewGuid() => Guid.Empty;
    }
}
