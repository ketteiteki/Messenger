using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiCommands.Conversations;

public record LeaveFromConversationCommand(
		Guid RequesterId,
		Guid ChatId) 
	: IRequest<Result<ChatDto>>;