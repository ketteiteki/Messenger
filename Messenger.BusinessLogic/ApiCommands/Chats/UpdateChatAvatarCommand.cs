using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Microsoft.AspNetCore.Http;

namespace Messenger.BusinessLogic.ApiCommands.Chats;

public record UpdateChatAvatarCommand(
		Guid RequesterId, 
		Guid ChatId, 
		IFormFile AvatarFile) 
	: IRequest<Result<ChatDto>>;
