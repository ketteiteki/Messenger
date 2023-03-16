using FluentValidation;
using Messenger.Domain.Entities.Validation;

namespace Messenger.Domain.Entities;

public class ChatUserEntity
{
	public Guid ChatId { get; set; }
	
	public ChatEntity Chat { get; set; }
	
	public Guid UserId { get; set; }

	public UserEntity User { get; set; }

	public bool CanSendMedia { get; set; } = true;

	public DateTime? MuteDateOfExpire { get; set; }
	
	public RoleUserByChatEntity Role { get; set; }

	public ChatUserEntity(Guid userId, Guid chatId, bool canSendMedia, DateTime? muteDateOfExpire)
	{
		UserId = userId;
		ChatId = chatId;
		CanSendMedia = canSendMedia;
		MuteDateOfExpire = muteDateOfExpire;
		
		new ChatUserEntityValidator().ValidateAndThrow(this);
	}

	public void UpdateMuteDateOfExpire(DateTime? muteDateOfExpire)
	{
		MuteDateOfExpire = muteDateOfExpire;
		new ChatUserEntityValidator().ValidateAndThrow(this);
	}
	
	public void UpdateCanSendMedia (bool canSendMedia)
	{
		CanSendMedia = canSendMedia;
		new ChatUserEntityValidator().ValidateAndThrow(this);
	}
}