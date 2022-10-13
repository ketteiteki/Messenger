using System.IdentityModel.Tokens.Jwt;
using Messenger.Domain.Entities;

namespace Messenger.Application.Interfaces;

public interface ITokenService
{
    public string CreateAccessToken(User user, string signKey = "secretAccessTokenKey_1231");

    public bool TryValidateAccessToken(string token, out JwtSecurityToken validatedJwtToken,
        string signKey = "secretAccessTokenKey_1231");
}