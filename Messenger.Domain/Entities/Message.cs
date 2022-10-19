using FluentValidation;
using Messenger.Domain.Entities.Abstraction;
using Messenger.Domain.Entities.Validation;

namespace Messenger.Domain.Entities;

public class Message : IBaseEntity
{
	public Guid Id { get; set; } = new Guid();

	public string Text { get; set; }
	
	public bool IsEdit { get; set; }
	
	public Guid? OwnerId { get; set; }

	public User? Owner { get; set; }

	public Guid? ReplyToMessageId { get; set; }
	
	public Message? ReplyToMessage { get; set; }
	
	public Guid ChatId { get; set; }

	public Chat Chat { get; set; }

	public List<Attachment> Attachments { get; set; } = new(); 

	public DateTime DateOfCreate { get; set; } = DateTime.UtcNow;
	
	public Chat? LastMessageByChat { get; set; }
	
	public List<DeletedMessageByUser> DeletedMessageByUsers { get; set; } = new();

	public Message(string text, Guid? ownerId, Guid? replyToMessageId, Guid chatId)
	{
		Text = text;
		OwnerId = ownerId;
		ReplyToMessageId = replyToMessageId;
		ChatId = chatId;

		new MessageValidator().ValidateAndThrow(this);
	}
}