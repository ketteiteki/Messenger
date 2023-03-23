using Messenger.BusinessLogic.Models;

namespace Messenger.BusinessLogic.Hubs;

public interface IChatHub
{
    Task BroadcastMessageAsync(MessageDto message);

    Task DeleteMessageAsync(MessageDeleteNotificationDto message);
    
    Task UpdateMessageAsync(MessageUpdateNotificationDto message);

    Task CreateDialogForInterlocutor(ChatDto chat);

    Task DeleteDialogForInterlocutor(string chatId);

    Task NotifyBanUser(NotifyBanUserDto notifyBanUserDto);

    Task NotifyPermissionForUser(NotifyPermissionForUserDto notifyPermissionForUserDto);

    Task CreateChatForUserAfterAddUserInChat(ChatDto chatDto);
}