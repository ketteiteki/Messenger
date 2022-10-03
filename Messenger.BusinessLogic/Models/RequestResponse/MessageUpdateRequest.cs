namespace Messenger.BusinessLogic.Models.RequestResponse;

public class MessageUpdateRequest
{
	public Guid Id { get; set; }
	
	public string Text { get; set; } = String.Empty;
}