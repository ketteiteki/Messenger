using Messenger.BusinessLogic.Models;

namespace Messenger.BusinessLogic.Hubs;

public interface IChatHub
{
    Task BroadcastMessageAsync(MessageDto message);

    Task DeleteMessageAsync(MessageDeleteNotificationDto message);
    
    Task UpdateMessageAsync(MessageUpdateNotificationDto message);
}