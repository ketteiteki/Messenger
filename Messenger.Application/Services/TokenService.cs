using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Messenger.Application.Interfaces;
using Messenger.Domain.Constants;
using Messenger.Domain.Entities;
using Microsoft.IdentityModel.Tokens;

namespace Messenger.Application.Services;

public class TokenService : ITokenService
{
    public string CreateAccessToken(User user, string signKey, int accessTokenLifetimeMinutes)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.Default.GetBytes(signKey);

        var claims = new[]
        {
            new Claim(ClaimConstants.Id, user.Id.ToString()),
            new Claim(ClaimConstants.Login, user.Nickname),
            new Claim(ClaimConstants.RandomToken, Guid.NewGuid().ToString())
        };

        var jwtToken = new JwtSecurityToken(
            expires: DateTime.UtcNow.AddMinutes(accessTokenLifetimeMinutes),
            claims: claims,
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256));

        return tokenHandler.WriteToken(jwtToken);
    }

    public bool TryValidateAccessToken(string token, string signKey, out JwtSecurityToken validatedJwtToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.Default.GetBytes(signKey);

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            validatedJwtToken = (JwtSecurityToken)validatedToken;

            return true;
        }
        catch
        {
            validatedJwtToken = new JwtSecurityToken();

            return false;
        }
    }
}