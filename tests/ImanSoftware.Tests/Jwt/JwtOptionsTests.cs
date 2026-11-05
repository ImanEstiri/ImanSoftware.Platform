using System.Security.Claims;
using ImanSoftware.Jwt;

namespace ImanSoftware.Tests.Jwt;

public class JwtOptionsTests
{
    [Fact]
    public void Defaults_HaveExpectedValues()
    {
        var options = new JwtOptions();

        Assert.Equal(string.Empty, options.SecretKey);
        Assert.Equal(string.Empty, options.Issuer);
        Assert.Equal(string.Empty, options.Audience);
        Assert.Equal(60, options.ExpiryMinutes);
        Assert.True(options.ValidateLifetime);
    }

    [Fact]
    public void Properties_AreSettable()
    {
        var options = new JwtOptions
        {
            SecretKey = "key",
            Issuer = "issuer",
            Audience = "audience",
            ExpiryMinutes = 15,
            ValidateLifetime = false
        };

        Assert.All(
            new (object? Actual, object? Expected)[]
            {
                (options.SecretKey, "key"),
                (options.Issuer, "issuer"),
                (options.Audience, "audience"),
                (options.ExpiryMinutes, 15),
                (options.ValidateLifetime, false)
            },
            pair => Assert.Equal(pair.Expected, pair.Actual));
    }
}

public class JwtTokenValidationResultTests
{
    [Fact]
    public void Success_CarriesPrincipal()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity("test"));

        var result = JwtTokenValidationResult.Success(principal);

        Assert.True(result.IsValid);
        Assert.Same(principal, result.Principal);
        Assert.Null(result.FailureReason);
    }

    [Fact]
    public void Failure_CarriesReason()
    {
        var result = JwtTokenValidationResult.Failure("token expired");

        Assert.False(result.IsValid);
        Assert.Null(result.Principal);
        Assert.Equal("token expired", result.FailureReason);
    }

    [Fact]
    public void Failure_AcceptsNullReason()
    {
        var result = JwtTokenValidationResult.Failure(null!);

        Assert.False(result.IsValid);
        Assert.Null(result.FailureReason);
    }
}
