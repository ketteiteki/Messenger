namespace Messenger.BusinessLogic.Models;

public class MessageDeleteNotificationDto
{
	public Guid? OwnerId { get; set; }

	public Guid ChatId { get; set; }

    public Guid MessageId { get; set; }
    
    public Guid? NewLastMessageId { get; init; }
	
    public string NewLastMessageText { get; init; }
	
    public string NewLastMessageAuthorDisplayName { get; init; }

    public DateTime? NewLastMessageDateOfCreate { get; init; }
}