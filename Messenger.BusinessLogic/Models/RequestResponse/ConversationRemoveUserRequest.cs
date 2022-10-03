namespace Messenger.BusinessLogic.Models.RequestResponse;

public class ConversationRemoveUserRequest
{
	public Guid ChatId { get; set; }

	public Guid UserId { get; set; }
}