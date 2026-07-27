using System.Security.Claims;

namespace ImanSoftware.Framework.Core.Jwt;

/// <summary>
/// Represents the result of validating a JWT token.
/// </summary>
public class JwtTokenValidationResult
{
    public bool IsValid { get; }
    public ClaimsPrincipal? Principal { get; }
    public string? FailureReason { get; }

    private JwtTokenValidationResult(bool isValid, ClaimsPrincipal? principal, string? failureReason)
    {
        IsValid = isValid;
        Principal = principal;
        FailureReason = failureReason;
    }

    public static JwtTokenValidationResult Success(ClaimsPrincipal principal)
        => new(true, principal, null);

    public static JwtTokenValidationResult Failure(string reason)
        => new(false, null, reason);
}