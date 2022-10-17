using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiCommands.Conversations;

public record UnbanUserInConversationCommand(
	Guid RequesterId,
	Guid ChatId,
	Guid UserId) 
	: IRequest<Result<UserDto>>;