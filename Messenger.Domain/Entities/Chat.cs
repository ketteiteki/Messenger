using FluentValidation;
using Messenger.Domain.Entities.Abstraction;
using Messenger.Domain.Entities.Validation;
using Messenger.Domain.Enum;

namespace Messenger.Domain.Entities;

public class Chat : IBaseEntity
{
	public Guid Id { get; set; } = Guid.NewGuid();

	public string? Name { get; set; }
	
	public string? Title { get; set; }

	public ChatType Type { get; set; }
	
	public Guid? OwnerId { get; set; }
	
	public User? Owner { get; set; }

	public string? AvatarLink { get; set; }
	
	public Guid? LastMessageId { get; set; }
	
	public Message? LastMessage { get; set; }
	
	public List<ChatUser> ChatUsers { get; set; } = new();
	
	public List<Message> Messages { get; set; } = new();

	public List<DeletedDialogByUser> DeletedDialogByUsers { get; set; } = new();

	public List<BanUserByChat> BanUserByChats { get; set; } = new();
	
	public Chat(string? name, string? title, ChatType type, Guid? ownerId, string? avatarLink, Guid? lastMessageId)
	{
		Name = name;
		Title = title;
		Type = type;
		OwnerId = ownerId;
		AvatarLink = avatarLink;
		LastMessageId = lastMessageId;
		
		new ChatValidator().ValidateAndThrow(this);
	}
}