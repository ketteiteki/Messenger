using Microsoft.AspNetCore.Http;

namespace Messenger.BusinessLogic.Models.RequestResponse;

public class MessageCreateRequest
{
	public string Text { get; set; }
	
	public Guid? ReplyToId { get; set; }
	
	public Guid ChatId { get; set; }
	
	public IFormFileCollection Files { get; set; }
}