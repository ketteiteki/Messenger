using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Enums;

namespace Messenger.BusinessLogic.ApiCommands.Conversations;

public record CreateOrUpdateRoleUserInConversationCommand(
		Guid RequesterId,
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