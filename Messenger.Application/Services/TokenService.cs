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
	public string CreateAccessToken(User user)
	{
		var tokenHandler = new JwtSecurityTokenHandler();
		var key = Encoding.Default.GetBytes(EnvironmentConstants.JwtIssuerSigningAccessKey);
		
		var jwtToken = new JwtSecurityToken(
			claims: new[]
			{
				new Claim(ClaimConstants.Id, user.Id.ToString()),
				new Claim(ClaimConstants.Login, user.NickName)
			},
			signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256));

		return tokenHandler.WriteToken(jwtToken);
	}
	
	public bool TryValidateAccessToken(string token, out JwtSecurityToken validatedJwtToken)
	{
		var tokenHandler = new JwtSecurityTokenHandler();
		var key = Encoding.Default.GetBytes(EnvironmentConstants.JwtIssuerSigningAccessKey);
		
		try
		{
			tokenHandler.ValidateToken(token, new TokenValidationParameters
			{
				ValidateAudience = false,
				ValidateIssuer = false,
				ValidateIssuerSigningKey = true,
				ValidateLifetime = false,
				IssuerSigningKey = new SymmetricSecurityKey(key),
				ClockSkew = TimeSpan.Zero
			}, out SecurityToken validatedToken);

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