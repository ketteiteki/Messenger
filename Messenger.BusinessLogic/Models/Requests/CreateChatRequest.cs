using Messenger.BusinessLogic.Models.Enum;
using Microsoft.AspNetCore.Http;

namespace Messenger.BusinessLogic.Models.Requests;

public class CreateChatRequest
{
	public string Name { get; set; }

	public string Title { get; set; }
	
	public CreateChatType Type { get; set; }
	
	public IFormFile AvatarFile { get; set; }
}