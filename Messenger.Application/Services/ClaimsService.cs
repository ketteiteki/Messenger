using System.Security.Claims;
using Messenger.Application.Interfaces;
using Messenger.Domain.Constants;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Messenger.Application.Services;

public class ClaimsService : IClaimsService
{
    public ClaimsPrincipal CreateSignInClaims(Guid userId)
    {
        var claims = new List<Claim>
        {
            new (ClaimConstants.Id, userId.ToString()),
            new (ClaimConstants.SessionId, Guid.NewGuid().ToString())
        };
		
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        
        return new ClaimsPrincipal(claimsIdentity);
    }
}