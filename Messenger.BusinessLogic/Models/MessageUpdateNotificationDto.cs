namespace Messenger.BusinessLogic.Models;

public class MessageUpdateNotificationDto
{
    public Guid? OwnerId { get; set; }

    public Guid ChatId { get; set; }

    public Guid MessageId { get; set; }
    
    public string UpdatedText { get; set; }
    
    public bool IsLastMessage {get; set; }
}