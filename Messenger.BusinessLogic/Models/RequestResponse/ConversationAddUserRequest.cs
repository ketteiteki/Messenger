namespace Messenger.BusinessLogic.Models.RequestResponse;

public class ConversationAddUserRequest
{
	public Guid ChatId { get; set; }

	public Guid UserId { get; set; }
}