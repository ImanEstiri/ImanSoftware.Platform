
using System.Security.Claims;

namespace ImanSoftware.Jwt;

public interface IJwtTokenService
{
    string GenerateToken(IEnumerable<Claim> claims);
    JwtTokenValidationResult ValidateToken(string token); // ارجاع به Core
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}