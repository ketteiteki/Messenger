using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Enum;

namespace Messenger.BusinessLogic.ApiCommands.Conversations;

public record CreateOrUpdateRoleUserInConversationCommand(
		Guid RequestorId,
		Guid ChatId,
		Guid UserId,
		string RoleTitle,
		RoleColor RoleColor,
		bool CanBanUser,
		bool CanChangeChatData,
		bool CanAddAndRemoveUserToConversation,
		bool CanGivePermissionToUser
	)
	: IRequest<Result<RoleUserByChatDto>>;