using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.Domain.Enum;

namespace Messenger.BusinessLogic.Conversations.Commands;

public class CreateOrUpdateRoleUserInConversationCommand : IRequest<RoleUserByChatDto>
{
	public Guid RequesterId { get; set; }

	public Guid ChatId { get; set; }

	public Guid UserId { get; set; }
	
	public string RoleTitle { get; set; } = null!;
	
	public RoleColor RoleColor { get; set; }

	public bool CanBanUser { get; set; }
	
	public bool CanChangeChatData { get; set; }
	
	public bool CanAddAndRemoveUserToConversation { get; set; }
	
	public bool CanGivePermissionToUser { get; set; }
}