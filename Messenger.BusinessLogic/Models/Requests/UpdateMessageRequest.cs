namespace Messenger.BusinessLogic.Models.Requests;

public class UpdateMessageRequest
{
	public Guid Id { get; set; }
	
	public string Text { get; set; }
}