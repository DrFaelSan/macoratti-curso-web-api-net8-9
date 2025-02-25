using APICatalogo.Configuracoes;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
namespace APICatalogo.Services;

public class TokenService : ITokenService
{
    private readonly JwtSection _jwtOptions;
    public TokenService(IOptions<JwtSection> jwtOptions) 
        => _jwtOptions = jwtOptions.Value;
    
    public JwtSecurityToken GenerateAcessToken(IEnumerable<Claim> claims)
    {
        var privateKey = Encoding.UTF8.GetBytes(_jwtOptions.SecretKey ?? throw new InvalidOperationException("Invalid key"));

        SigningCredentials signingCredentials = new(new SymmetricSecurityKey(privateKey), SecurityAlgorithms.HmacSha256Signature);

        var nowDateTime = DateTime.UtcNow;
        SecurityTokenDescriptor tokenDescriptior = new()
        {
            Subject = new ClaimsIdentity(claims),
            NotBefore = nowDateTime.AddMinutes(1),
            Expires = nowDateTime.AddMinutes(_jwtOptions.TokenValidityInMinutes),
            Audience = _jwtOptions.ValidAudience,
            Issuer = _jwtOptions.ValidIssuer, 
            SigningCredentials = signingCredentials
        };

        JwtSecurityTokenHandler tokenHandler = new();
        var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptior);
        return token;

    }

    public string GenerateRefreshToken()
    {
        var secureRandomBytes = new byte[128];

        using var randomNumberGenerator = RandomNumberGenerator.Create();

        randomNumberGenerator.GetBytes(secureRandomBytes);

        var refreshToken = Convert.ToBase64String(secureRandomBytes);
        return refreshToken;
    }

    public ClaimsPrincipal GetClaimsPrincipalFromExpiredToken(string token)
    {
        TokenValidationParameters tokenValidationParameters = new()
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey ?? throw new InvalidOperationException("Invalid key"))),
            ValidateLifetime = false,
        };

        JwtSecurityTokenHandler tokenHandler = new();

        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

        if(securityToken is not JwtSecurityToken jwtSecurityToken || 
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");
        

        return principal;
    }
}
