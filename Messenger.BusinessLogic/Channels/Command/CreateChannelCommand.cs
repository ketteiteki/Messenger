using MediatR;
using Messenger.BusinessLogic.Models;
using Microsoft.AspNetCore.Http;

namespace Messenger.BusinessLogic.Channels.Command;

public class CreateChannelCommand : IRequest<ChatDto>
{
	public Guid RequesterId { get; set; }

	public string Name { get; set; }
	
	public string Title { get; set; }
	
	public IFormFile? AvatarFile { get; set; }
}

