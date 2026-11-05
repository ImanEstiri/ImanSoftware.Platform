using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ImanSoftware.Jwt;

namespace ImanSoftware.Tests.Jwt;

public class JwtTokenServiceTests
{
    private const string SecretKey = "test-secret-key-for-imansoftware-jwt-tests-must-be-at-least-64-bytes-long!!";
    private const string Issuer = "test-issuer";
    private const string Audience = "test-audience";

    private static JwtOptions CreateOptions(
        string? secretKey = null,
        string? issuer = null,
        string? audience = null,
        int expiryMinutes = 60,
        bool validateLifetime = true) => new()
    {
        SecretKey = secretKey ?? SecretKey,
        Issuer = issuer ?? Issuer,
        Audience = audience ?? Audience,
        ExpiryMinutes = expiryMinutes,
        ValidateLifetime = validateLifetime
    };

    private static JwtTokenService CreateService(JwtOptions? options = null) =>
        new(Options.Create(options ?? CreateOptions()));

    private static Claim[] DefaultClaims =>
    [
        new("sub", "42"),
        new("role", "admin")
    ];

    [Fact]
    public void GenerateToken_ProducesThreePartJws()
    {
        var token = CreateService().GenerateToken(DefaultClaims);

        Assert.Equal(3, token.Split('.').Length);
    }

    [Fact]
    public void GenerateToken_EncodesIssuerAndAudience()
    {
        var token = CreateService().GenerateToken(DefaultClaims);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        Assert.Equal(Issuer, jwt.Issuer);
        Assert.Contains(jwt.Audiences, a => a == Audience);
    }

    [Fact]
    public void ValidateToken_WithValidToken_ReturnsPrincipalWithMappedClaims()
    {
        var service = CreateService();
        var token = service.GenerateToken(DefaultClaims);

        var result = service.ValidateToken(token);

        Assert.True(result.IsValid);
        Assert.NotNull(result.Principal);
        Assert.Null(result.FailureReason);
        Assert.Equal("42", result.Principal!.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        Assert.Equal("admin", result.Principal.FindFirst(ClaimTypes.Role)?.Value);
    }

    [Fact]
    public void ValidateToken_WithTamperedSignature_ReturnsFailure()
    {
        var service = CreateService();
        var token = service.GenerateToken(DefaultClaims);
        var tampered = token[..^2] + (token.EndsWith("aa", StringComparison.Ordinal) ? "bb" : "aa");

        var result = service.ValidateToken(tampered);

        Assert.False(result.IsValid);
        Assert.Null(result.Principal);
        Assert.False(string.IsNullOrEmpty(result.FailureReason));
    }

    [Fact]
    public void ValidateToken_WithWrongIssuer_ReturnsFailure()
    {
        var token = CreateService().GenerateToken(DefaultClaims);
        var otherService = CreateService(CreateOptions(issuer: "other-issuer"));

        var result = otherService.ValidateToken(token);

        Assert.False(result.IsValid);
        Assert.False(string.IsNullOrEmpty(result.FailureReason));
    }

    [Fact]
    public void ValidateToken_WithWrongAudience_ReturnsFailure()
    {
        var token = CreateService().GenerateToken(DefaultClaims);
        var otherService = CreateService(CreateOptions(audience: "other-audience"));

        var result = otherService.ValidateToken(token);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void ValidateToken_WithWrongSigningKey_ReturnsFailure()
    {
        var token = CreateService().GenerateToken(DefaultClaims);
        var otherService = CreateService(CreateOptions(secretKey: SecretKey + "extra-padding-for-different-key"));

        var result = otherService.ValidateToken(token);

        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-a-jwt")]
    [InlineData("a.b.c")]
    public void ValidateToken_NeverThrows_ReturnsFailureForGarbage(string token)
    {
        var result = CreateService().ValidateToken(token);

        Assert.False(result.IsValid);
        Assert.Null(result.Principal);
    }

    [Fact]
    public void ValidateToken_WithExpiredToken_ReturnsFailureWhenLifetimeIsValidated()
    {
        var expiredService = CreateService(CreateOptions(expiryMinutes: -5));
        var token = expiredService.GenerateToken(DefaultClaims);

        var result = expiredService.ValidateToken(token);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void ValidateToken_WithExpiredToken_SucceedsWhenLifetimeValidationDisabled()
    {
        var expiredService = CreateService(CreateOptions(expiryMinutes: -5));
        var token = expiredService.GenerateToken(DefaultClaims);
        var lenientService = CreateService(CreateOptions(expiryMinutes: -5, validateLifetime: false));

        var result = lenientService.ValidateToken(token);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void GetPrincipalFromExpiredToken_ReturnsClaimsFromExpiredToken()
    {
        var expiredService = CreateService(CreateOptions(expiryMinutes: -5));
        var token = expiredService.GenerateToken(DefaultClaims);

        var principal = expiredService.GetPrincipalFromExpiredToken(token);

        Assert.NotNull(principal);
        Assert.Equal("42", principal!.FindFirst(ClaimTypes.NameIdentifier)?.Value);
    }

    [Fact]
    public void GetPrincipalFromExpiredToken_WithWrongSignature_ReturnsNull()
    {
        var token = CreateService().GenerateToken(DefaultClaims);
        var otherService = CreateService(CreateOptions(secretKey: SecretKey + "extra-padding-for-different-key"));

        Assert.Null(otherService.GetPrincipalFromExpiredToken(token));
    }

    [Fact]
    public void GetPrincipalFromExpiredToken_WithGarbage_ReturnsNull()
    {
        Assert.Null(CreateService().GetPrincipalFromExpiredToken("garbage"));
    }

    [Fact]
    public void GenerateToken_UsesHS256Algorithm()
    {
        var token = CreateService().GenerateToken(DefaultClaims);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        Assert.Equal(SecurityAlgorithms.HmacSha256, jwt.SignatureAlgorithm);
    }

    [Fact]
    public void GenerateToken_ExpiryMatchesConfiguredMinutes()
    {
        var utcBefore = DateTime.UtcNow;
        var token = CreateService(CreateOptions(expiryMinutes: 30)).GenerateToken(DefaultClaims);
        var utcAfter = DateTime.UtcNow;

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        Assert.InRange(
            jwt.ValidTo,
            utcBefore.AddMinutes(30).AddSeconds(-5),
            utcAfter.AddMinutes(30).AddSeconds(5));
    }
}
