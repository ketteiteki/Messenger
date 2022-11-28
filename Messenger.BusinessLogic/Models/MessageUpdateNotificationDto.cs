namespace Messenger.BusinessLogic.Models;

public class MessageUpdateNotificationDto
{
    public Guid MessageId { get; set; }
    
    public string UpdatedText { get; set; }
    
    public bool IsLastMessage {get; set; }
}