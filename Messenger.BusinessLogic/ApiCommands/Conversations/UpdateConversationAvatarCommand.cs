using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Microsoft.AspNetCore.Http;

namespace Messenger.BusinessLogic.ApiCommands.Conversations;

public record UpdateConversationAvatarCommand(
		Guid RequestorId, 
		Guid ChatId, 
		IFormFile? AvatarFile) 
	: IRequest<Result<ChatDto>>;
