using Microsoft.AspNetCore.SignalR;

namespace Messenger.BusinessLogic.Hubs;

public class ChatHub : Hub<IChatHub>
{
    public Task JoinChat(Guid chatId)
    {
        return Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
    }
}