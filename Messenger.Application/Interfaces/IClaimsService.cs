using System.Security.Claims;

namespace Messenger.Application.Interfaces;

public interface IClaimsService
{
    public ClaimsPrincipal CreateSignInClaims(Guid userId);
}