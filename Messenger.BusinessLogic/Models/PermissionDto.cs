using Messenger.Domain.Entities;

namespace Messenger.BusinessLogic.Models;

public class PermissionDto
{
	public Guid UserId { get; private set; }

	public Guid ChatId { get; private set; }
	
	public bool CanSendMedia { get; private set; }

	public DateTime? MuteDateOfExpire { get; private set; }

	public PermissionDto(ChatUserEntity chatUser)
	{
		UserId = chatUser.UserId;
		ChatId = chatUser.ChatId;
		CanSendMedia = chatUser.CanSendMedia;
		MuteDateOfExpire = chatUser.MuteDateOfExpire;
	}
}