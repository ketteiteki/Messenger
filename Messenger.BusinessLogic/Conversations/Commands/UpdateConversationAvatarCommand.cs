using MediatR;
using Messenger.BusinessLogic.Models;
using Microsoft.AspNetCore.Http;

namespace Messenger.BusinessLogic.Conversations.Commands;

public class UpdateConversationAvatarCommand : IRequest<ChatDto>
{
	public Guid RequesterId { get; set; }
	
	public Guid ChatId { get; set; }
	
	public IFormFile? AvatarFile { get; set; }
}