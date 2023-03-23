using System.IdentityModel.Tokens.Jwt;
using Messenger.Domain.Entities;

namespace Messenger.Application.Interfaces;

public interface ITokenService
{
    public string CreateAccessToken(UserEntity user, string signKey, int accessTokenLifetimeMinutes);

    public bool TryValidateAccessToken(string token, string signKey, out JwtSecurityToken validatedJwtToken);
}