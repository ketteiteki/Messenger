using FluentValidation;
using Messenger.Domain.Entities.Validation;

namespace Messenger.Domain.Entities;

public class DeletedMessageByUserEntity
{
	public Guid MessageId { get; set; }
	
	public MessageEntity Message { get; set; }

	public Guid UserId { get; set; }

	public UserEntity User { get; set; }

	public DeletedMessageByUserEntity(Guid messageId, Guid userId)
	{
		MessageId = messageId;
		UserId = userId;
		
		new DeletedMessageByUserEntityValidator().ValidateAndThrow(this);
	}
}