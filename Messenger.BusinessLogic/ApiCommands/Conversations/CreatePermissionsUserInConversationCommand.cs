using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiCommands.Conversations;

public record CreatePermissionsUserInConversationCommand(
	Guid RequesterId,
	Guid ChatId,
	Guid UserId,
	bool CanSendMedia,
	int? MuteMinutes)
	: IRequest<Result<PermissionDto>>;
