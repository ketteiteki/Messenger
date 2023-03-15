using FluentValidation;
using Messenger.Domain.Entities.Abstraction;
using Messenger.Domain.Entities.Validation;

namespace Messenger.Domain.Entities;

public class MessageEntity : IBaseEntity
{
	public Guid Id { get; set; } = Guid.NewGuid();

	public string Text { get; set; }
	
	public bool IsEdit { get; set; }
	
	public Guid? OwnerId { get; set; }

	public UserEntity? Owner { get; set; }

	public Guid? ReplyToMessageId { get; set; }
	
	public MessageEntity? ReplyToMessage { get; set; }
	
	public Guid ChatId { get; set; }

	public ChatEntity Chat { get; set; }

	public List<AttachmentEntity> Attachments { get; set; } = new(); 

	public DateTime DateOfCreate { get; set; } = DateTime.UtcNow;
	
	public ChatEntity? LastMessageByChat { get; set; }

	public List<DeletedMessageByUserEntity> DeletedMessageByUsers { get; set; }

	public MessageEntity(string text, Guid? ownerId, Guid? replyToMessageId, Guid chatId)
	{
		Text = text;
		OwnerId = ownerId;
		ReplyToMessageId = replyToMessageId;
		ChatId = chatId;

		new MessageEntityValidator().ValidateAndThrow(this);
	}
}