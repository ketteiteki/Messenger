using Microsoft.AspNetCore.Http;

namespace Messenger.BusinessLogic.Models.Requests;

public class CreateMessageRequest
{
	public string Text { get; set; }
	
	public Guid? ReplyToId { get; set; }
	
	public Guid ChatId { get; set; }
	
	public IFormFileCollection Files { get; set; }
}