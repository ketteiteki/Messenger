namespace Messenger.BusinessLogic.Models.Requests;

public class UpdateChatDataRequest
{
	public Guid ChatId { get; set; }
	
	public string Title { get; set; }

	public string Name { get; set; }
}