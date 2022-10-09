using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiCommands.Conversations;

public record CreatePermissionsUserInConversationCommand(
	Guid RequestorId,
	Guid ChatId,
	Guid UserId,
	bool CanSendMedia)
	: IRequest<Result<PermissionDto>>;
