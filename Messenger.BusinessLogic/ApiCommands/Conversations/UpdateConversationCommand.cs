using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiCommands.Conversations;

public record UpdateConversationCommand(
	Guid RequestorId,
	Guid ChatId,
	string Name,
	string Title
	) : IRequest<Result<ChatDto>>;