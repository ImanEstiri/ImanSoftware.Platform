using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ImanSoftware.CurrentUserAccessor;

namespace ImanSoftware.Tests.CurrentUserAccessor;

public class HttpContextUserAccessorTests
{
    private sealed class StubHttpContextAccessor : IHttpContextAccessor
    {
        public HttpContext? HttpContext { get; set; }
    }

    private static StubHttpContextAccessor CreateAccessorWithSubClaim(string? subValue)
    {
        var httpContext = new DefaultHttpContext();
        if (subValue is not null)
        {
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(
                [new Claim("sub", subValue)],
                authenticationType: "test"));
        }

        return new StubHttpContextAccessor { HttpContext = httpContext };
    }

    [Fact]
    public void GetCurrentUser_ReturnsHttpContextUser()
    {
        var accessor = CreateAccessorWithSubClaim("42");

        var accessorOfInt = new HttpContextUserAccessor<int>(accessor);

        Assert.NotNull(accessorOfInt.GetCurrentUser());
        Assert.True(accessorOfInt.GetCurrentUser()!.HasClaim("sub", "42"));
    }

    [Fact]
    public void GetCurrentUser_WithoutHttpContext_ReturnsNull()
    {
        var accessor = new HttpContextUserAccessor<int>(new StubHttpContextAccessor());

        Assert.Null(accessor.GetCurrentUser());
    }

    [Fact]
    public void GetCurrentUserId_WithIntSubClaim_ConvertsToId()
    {
        var accessor = new HttpContextUserAccessor<int>(CreateAccessorWithSubClaim("42"));

        Assert.Equal(42, accessor.GetCurrentUserId());
    }

    [Fact]
    public void GetCurrentUserId_WithGuidSubClaim_ConvertsToId()
    {
        var id = Guid.NewGuid();
        var accessor = new HttpContextUserAccessor<Guid>(CreateAccessorWithSubClaim(id.ToString()));

        Assert.Equal(id, accessor.GetCurrentUserId());
    }

    [Fact]
    public void GetCurrentUserId_WithStringUserId_ReturnsClaimValue()
    {
        var accessor = new HttpContextUserAccessor<string>(CreateAccessorWithSubClaim("user-1"));

        Assert.Equal("user-1", accessor.GetCurrentUserId());
    }

    [Fact]
    public void GetCurrentUserId_WithLongSubClaim_ConvertsToId()
    {
        var accessor = new HttpContextUserAccessor<long>(CreateAccessorWithSubClaim("9876543210"));

        Assert.Equal(9876543210L, accessor.GetCurrentUserId());
    }

    [Fact]
    public void GetCurrentUserId_WithMalformedValue_ReturnsDefault()
    {
        var accessor = new HttpContextUserAccessor<int>(CreateAccessorWithSubClaim("not-a-number"));

        Assert.Equal(0, accessor.GetCurrentUserId());
    }

    [Fact]
    public void GetCurrentUserId_WithoutSubClaim_ReturnsDefault()
    {
        var accessor = new HttpContextUserAccessor<int>(CreateAccessorWithSubClaim(null));

        Assert.Equal(0, accessor.GetCurrentUserId());
    }

    [Fact]
    public void GetCurrentUserId_WithEmptySubClaim_ReturnsDefault()
    {
        var accessor = new HttpContextUserAccessor<int>(CreateAccessorWithSubClaim(string.Empty));

        Assert.Equal(0, accessor.GetCurrentUserId());
    }

    [Fact]
    public void GetCurrentUserId_WithoutHttpContext_ReturnsDefault()
    {
        var accessor = new HttpContextUserAccessor<int>(new StubHttpContextAccessor());

        Assert.Equal(0, accessor.GetCurrentUserId());
    }

    [Fact]
    public void GetCurrentUserId_WithUnsupportedUserIdType_ReturnsDefault()
    {
        var accessor = new HttpContextUserAccessor<object>(CreateAccessorWithSubClaim("42"));

        Assert.Null(accessor.GetCurrentUserId());
    }

    [Fact]
    public void GetCurrentUserId_WithDifferentClaimType_ReturnsDefault()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim(ClaimTypes.NameIdentifier, "42")],
            authenticationType: "test"));

        var accessor = new HttpContextUserAccessor<int>(new StubHttpContextAccessor { HttpContext = httpContext });

        Assert.Equal(0, accessor.GetCurrentUserId());
    }
}
