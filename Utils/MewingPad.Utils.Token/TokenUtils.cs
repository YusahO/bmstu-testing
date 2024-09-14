using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace MewingPad.Utils.Token;

public static class TokenUtils
{
    public static string GenerateAccessToken(string userId, string userEmail, string userName)
    {
        var expires = DateTime.UtcNow.Add(TimeSpan.FromSeconds(AccessTokenOptions.LIFETIME));

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Email, userEmail),
            new(ClaimTypes.Name, userName)
        };

        var securityKey = AccessTokenOptions.GetSymmetricSecurityKey();

        var jwt = new JwtSecurityToken(
            issuer: AccessTokenOptions.ISSUER,
            audience: AccessTokenOptions.AUDIENCE,
            claims: claims,
            expires: expires,
            signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    public static string RegenerateAccessToken(string oldToken)
    {
        var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(oldToken) ?? throw new Exception();

        var oldClaims = jwtToken.Claims.ToList();

        var expires = DateTime.UtcNow.Add(TimeSpan.FromSeconds(AccessTokenOptions.LIFETIME));
        var claims = new List<Claim>{
            new(ClaimTypes.NameIdentifier, oldClaims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value),
            new(ClaimTypes.Email, oldClaims.FirstOrDefault(c => c.Type == ClaimTypes.Email)!.Value),
            new(ClaimTypes.Name, oldClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name)!.Value)
        };

        var securityKey = AccessTokenOptions.GetSymmetricSecurityKey();

        var jwt = new JwtSecurityToken(
            issuer: AccessTokenOptions.ISSUER,
            audience: AccessTokenOptions.AUDIENCE,
            claims: claims,
            expires: expires,
            signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    public static string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];

        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public static bool IsAccessTokenExpired(string accessToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        if (!tokenHandler.CanReadToken(accessToken))
        {
            return false;
        }

        if (tokenHandler.ReadToken(accessToken) is not JwtSecurityToken jwtToken)
        {
            return false;
        }

        var exp = jwtToken.Payload.Expiration;
        if (exp is null)
        {
            return false;
        }
        
        var expirationTime = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(exp)).DateTime;
        if (expirationTime < DateTime.UtcNow)
        {
            return true;
        }

        return false;
    }

    public static string TryGetClaimOfType(string token, string type = ClaimTypes.NameIdentifier)
    {
        var jwtToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken ?? throw new Exception();
        var claims = jwtToken.Claims.ToList();
        return claims.FirstOrDefault(c => c.Type == type)!.Value;
    }
}
