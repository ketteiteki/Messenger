using Messenger.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Messenger.BusinessLogic.Hubs;

[Authorize]
public class ChatHub : Hub<IChatHub>
{
    public Task JoinChat(Guid chatId)
    {
        return Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
    }
    
    public Task LeaveChat(Guid chatId)
    {
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId.ToString());
    }
}