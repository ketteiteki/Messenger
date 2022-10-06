using Messenger.Domain.Entities;
using Messenger.Domain.Enum;

namespace Messenger.BusinessLogic.Models;

public class RoleUserByChatDto
{
	public Guid UserId { get; set; }

	public Guid ChatId { get; set; }
	
	public string RoleTitle { get; set; }
	
	public RoleColor RoleColor { get; set; }
	
	public bool CanBanUser { get; set; }
	
	public bool CanChangeChatData { get; set; }

	public bool CanGivePermissionToUser { get; set; }
	
	public bool CanAddAndRemoveUserToConversation { get; set; }
	
	public bool IsOwner { get; set; }
	
	public RoleUserByChatDto(RoleUserByChat roleUserByChat)
	{
		UserId = roleUserByChat.UserId;
		ChatId = roleUserByChat.ChatId;
		RoleTitle = roleUserByChat.RoleTitle;
		RoleColor = roleUserByChat.RoleColor;
		CanBanUser = roleUserByChat.CanBanUser;
		CanChangeChatData = roleUserByChat.CanChangeChatData;
		CanGivePermissionToUser = roleUserByChat.CanGivePermissionToUser;
		CanAddAndRemoveUserToConversation = roleUserByChat.CanAddAndRemoveUserToConversation;
		IsOwner = roleUserByChat.IsOwner;
	}
}