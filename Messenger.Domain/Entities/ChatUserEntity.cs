namespace Messenger.Domain.Entities;

public class ChatUserEntity
{
	public Guid ChatId { get; set; }
	
	public ChatEntity Chat { get; set; }
	
	public Guid UserId { get; set; }

	public UserEntity User { get; set; }

	public bool CanSendMedia { get; set; } = true;

	public DateTime? MuteDateOfExpire { get; set; }
	
	public RoleUserByChatEntity? Role { get; set; }

	public ChatUserEntity(Guid userId, Guid chatId, bool canSendMedia, DateTime? muteDateOfExpire)
	{
		UserId = userId;
		ChatId = chatId;
		CanSendMedia = canSendMedia;
		MuteDateOfExpire = muteDateOfExpire;
	}
}