using FluentValidation;
using Messenger.Domain.Entities.Validation;

namespace Messenger.Domain.Entities;

public class BanUserByChatEntity
{
	public Guid UserId { get; set; }

	public UserEntity User { get; set; }
	
	public Guid ChatId { get; set; }
	
	public ChatEntity Chat { get; set; }
	
	public DateTime BanDateOfExpire { get; set; }

	public BanUserByChatEntity(Guid userId, Guid chatId, DateTime banDateOfExpire)
	{
		UserId = userId;
		ChatId = chatId;
		BanDateOfExpire = banDateOfExpire;

		new BanUserByChatEntityValidator().ValidateAndThrow(this);
	}
}
