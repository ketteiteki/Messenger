using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiCommands.Conversations;

public record JoinToConversationCommand(
	Guid RequestorId, 
	Guid ChatId
	) : IRequest<Result<ChatDto>>;