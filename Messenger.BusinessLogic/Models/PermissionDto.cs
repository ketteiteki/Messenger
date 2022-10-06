using Messenger.Domain.Entities;

namespace Messenger.BusinessLogic.Models;

public class PermissionDto
{
	public bool CanSendMedia { get; set; }

	public DateTime? MuteDateOfExpire { get; set; }

	public PermissionDto(ChatUser chatUser)
	{
		CanSendMedia = chatUser.CanSendMedia;
		MuteDateOfExpire = chatUser.MuteDateOfExpire;
	}
}