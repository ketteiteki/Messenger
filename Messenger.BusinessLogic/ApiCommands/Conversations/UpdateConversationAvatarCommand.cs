using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Microsoft.AspNetCore.Http;

namespace Messenger.BusinessLogic.ApiCommands.Conversations;

public record UpdateConversationAvatarCommand(
		Guid RequesterId, 
		Guid ChatId, 
		IFormFile AvatarFile) 
	: IRequest<Result<ChatDto>>;
