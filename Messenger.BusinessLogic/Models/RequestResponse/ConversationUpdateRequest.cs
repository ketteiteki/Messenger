using Microsoft.AspNetCore.Http;

namespace Messenger.BusinessLogic.Models.RequestResponse;

public class ConversationUpdateRequest
{
	public Guid Id { get; set; }
	
	public string DisplayName { get; set; } = null!;

	public string Nickname { get; set; } = null!;
	
	public IFormFile? AvatarFile { get; set; }
}