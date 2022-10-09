using System.IdentityModel.Tokens.Jwt;
using Messenger.Domain.Entities;

namespace Messenger.Application.Interfaces;

public interface ITokenService
{
	public string CreateAccessToken(User user);

	public bool TryValidateAccessToken(string token, out JwtSecurityToken validatedJwtToken);
}