using Messenger.Domain.Constants;
using Microsoft.AspNetCore.SignalR;

namespace Messenger.BusinessLogic.Hubs.Providers;

public class CustomUserIdProvider : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        return connection.User.Claims.First(x => x.Type == ClaimConstants.Id).Value;
    }
}