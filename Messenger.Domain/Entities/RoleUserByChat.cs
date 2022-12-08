using FluentValidation;
using Messenger.Domain.Entities.Validation;
using Messenger.Domain.Enum;

namespace Messenger.Domain.Entities;

public class RoleUserByChat
{
	public Guid UserId { get; set; }
	
	public User User { get; set; }

	public Guid ChatId { get; set; }
	
	public ChatUser ChatUser { get; set; }
	
	public string RoleTitle { get; set; }
	
	public RoleColor RoleColor { get; set; }
	
	public bool CanBanUser { get; set; }
	
	public bool CanChangeChatData { get; set; }

	public bool CanGivePermissionToUser { get; set; }
	
	public bool CanAddAndRemoveUserToConversation { get; set; }

	public bool IsOwner { get; set; }

	public RoleUserByChat(Guid userId, Guid chatId, string roleTitle, RoleColor roleColor,
		bool canBanUser, bool canChangeChatData, bool canGivePermissionToUser,
		bool canAddAndRemoveUserToConversation, bool isOwner)
	{
		UserId = userId;
		ChatId = chatId;
		RoleTitle = roleTitle;
		RoleColor = roleColor;
		CanBanUser = canBanUser;
		CanChangeChatData = canChangeChatData;
		CanGivePermissionToUser = canGivePermissionToUser;
		CanAddAndRemoveUserToConversation = canAddAndRemoveUserToConversation;
		IsOwner = isOwner;
		
		new RoleUserByChatValidator().ValidateAndThrow(this);
	}
}