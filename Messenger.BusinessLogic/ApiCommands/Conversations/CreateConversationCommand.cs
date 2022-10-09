using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Microsoft.AspNetCore.Http;

namespace Messenger.BusinessLogic.ApiCommands.Conversations;

public record CreateConversationCommand(
		Guid RequestorId,
		string Name,
		string Title,
		IFormFile? AvatarFile)
	: IRequest<Result<ChatDto>>;
