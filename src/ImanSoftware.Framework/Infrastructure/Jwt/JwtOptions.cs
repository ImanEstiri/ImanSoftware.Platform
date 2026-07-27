namespace ImanSoftware.Framework.Infrastructure.Jwt;

/// <summary>
/// Configuration options for JWT authentication.
/// </summary>
public class JwtOptions
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; } = 60;
    public bool ValidateLifetime { get; set; } = true;
}