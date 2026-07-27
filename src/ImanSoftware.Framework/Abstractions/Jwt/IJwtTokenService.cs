
using ImanSoftware.Framework.Core.Jwt;
using System.Security.Claims;

namespace ImanSoftware.Framework.Abstractions.Jwt;

public interface IJwtTokenService
{
    string GenerateToken(IEnumerable<Claim> claims);
    JwtTokenValidationResult ValidateToken(string token); // ارجاع به Core
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}