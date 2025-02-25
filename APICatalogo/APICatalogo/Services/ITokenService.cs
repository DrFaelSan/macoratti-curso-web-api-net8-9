using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace APICatalogo.Services;

public interface ITokenService
{
    JwtSecurityToken GenerateAcessToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
    ClaimsPrincipal GetClaimsPrincipalFromExpiredToken(string token);
}
