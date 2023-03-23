namespace Messenger.BusinessLogic.Models;

public class NotifyPermissionForUserDto
{
    public Guid ChatId { get; set; }

    public bool CanSendMedia { get; set; }

    public DateTime? MuteDateOfExpire { get; set; }

    public NotifyPermissionForUserDto(Guid chatId, bool canSendMedia, DateTime? muteDateOfExpire)
    {
        ChatId = chatId;
        CanSendMedia = canSendMedia;
        MuteDateOfExpire = muteDateOfExpire;
    }
}