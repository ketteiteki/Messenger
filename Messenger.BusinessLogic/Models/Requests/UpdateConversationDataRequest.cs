namespace Messenger.BusinessLogic.Models.Requests;

public class UpdateConversationDataRequest
{
	public Guid ChatId { get; set; }
	
	public string Title { get; set; } = null!;

	public string Name { get; set; } = null!;
}