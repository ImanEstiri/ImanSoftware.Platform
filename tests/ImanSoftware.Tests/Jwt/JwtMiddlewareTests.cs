using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using ImanSoftware.AspNetCore.Middlewares;
using ImanSoftware.Jwt;

namespace ImanSoftware.Tests.Jwt;

public class JwtMiddlewareTests
{
    private readonly IJwtTokenService _tokenService = Substitute.For<IJwtTokenService>();

    private static ClaimsPrincipal CreatePrincipal(string userId = "42") =>
        new(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, userId)], "authenticated"));

    private static (DefaultHttpContext Context, Task NextResult) CreateContext(string? authorizationHeader)
    {
        var context = new DefaultHttpContext();
        if (authorizationHeader is not null)
        {
            context.Request.Headers["Authorization"] = authorizationHeader;
        }

        return (context, Task.CompletedTask);
    }

    private static async Task<DefaultHttpContext> InvokeAsync(
        IJwtTokenService tokenService,
        string? authorizationHeader,
        Func<Task>? next = null)
    {
        var (context, _) = CreateContext(authorizationHeader);
        var nextCalled = false;
        var middleware = new JwtMiddleware(
            _ => { nextCalled = true; return Task.CompletedTask; },
            tokenService);

        await middleware.InvokeAsync(context);

        Assert.True(nextCalled, "The next middleware must always run.");
        return context;
    }

    [Fact]
    public async Task InvokeAsync_WithValidBearerToken_SetsContextUser()
    {
        var principal = CreatePrincipal();
        _tokenService.ValidateToken("good-token").Returns(JwtTokenValidationResult.Success(principal));

        var context = await InvokeAsync(_tokenService, "Bearer good-token");

        Assert.Same(principal, context.User);
    }

    [Fact]
    public async Task InvokeAsync_WithLowercaseScheme_StillExtractsToken()
    {
        var principal = CreatePrincipal();
        _tokenService.ValidateToken("good-token").Returns(JwtTokenValidationResult.Success(principal));

        var context = await InvokeAsync(_tokenService, "bearer good-token");

        Assert.Same(principal, context.User);
    }

    [Fact]
    public async Task InvokeAsync_WithInvalidToken_KeepsDefaultUserAndStillCallsNext()
    {
        _tokenService.ValidateToken("bad-token").Returns(JwtTokenValidationResult.Failure("invalid"));

        var context = await InvokeAsync(_tokenService, "Bearer bad-token");

        Assert.False(context.User.HasClaim(c => c.Type == ClaimTypes.NameIdentifier));
    }

    [Fact]
    public async Task InvokeAsync_WithoutAuthorizationHeader_DoesNotCallValidateToken()
    {
        await InvokeAsync(_tokenService, null);

        _tokenService.DidNotReceive().ValidateToken(Arg.Any<string>());
    }

    [Theory]
    [InlineData("Bearer")]
    [InlineData("Bearer ")]
    [InlineData("Bearer one two")]
    [InlineData("Basic dXNlcjpwYXNz")]
    [InlineData("Bearer  double-spaced")]
    public async Task InvokeAsync_WithMalformedHeader_DoesNotCallValidateToken(string header)
    {
        await InvokeAsync(_tokenService, header);

        _tokenService.DidNotReceive().ValidateToken(Arg.Any<string>());
    }

    [Fact]
    public async Task InvokeAsync_PassesExtractedTokenOnly()
    {
        _tokenService.ValidateToken("extracted").Returns(JwtTokenValidationResult.Failure("nope"));

        await InvokeAsync(_tokenService, "Bearer extracted");

        _tokenService.Received(1).ValidateToken("extracted");
    }
}

public class UseMiddlewareExtensionsTests
{
    [Fact]
    public void UseJwtAuthentication_WithRegisteredService_UsesMiddlewareAndReturnsBuilder()
    {
        var services = new ServiceCollection();
        services.AddSingleton(Substitute.For<IJwtTokenService>());
        var app = Substitute.For<IApplicationBuilder>();
        app.ApplicationServices.Returns(services.BuildServiceProvider());
        app.Use(Arg.Any<Func<RequestDelegate, RequestDelegate>>()).Returns(app);

        var result = app.UseJwtAuthentication();

        Assert.Same(app, result);
        // UseMiddleware<T> is an extension method — it surfaces as a Use(...) call on the builder.
        app.Received(1).Use(Arg.Any<Func<RequestDelegate, RequestDelegate>>());
    }

    [Fact]
    public void UseJwtAuthentication_WithoutRegisteredService_ThrowsInvalidOperationException()
    {
        var services = new ServiceCollection();
        var app = Substitute.For<IApplicationBuilder>();
        app.ApplicationServices.Returns(services.BuildServiceProvider());

        var exception = Assert.Throws<InvalidOperationException>(() => app.UseJwtAuthentication());

        Assert.Equal("IJwtTokenService is not registered in the DI container. ", exception.Message);
    }
}
