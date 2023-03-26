using FluentValidation;
using Messenger.Domain.Entities.Abstraction;
using Messenger.Domain.Entities.Validation;
using Messenger.Domain.Enums;

namespace Messenger.Domain.Entities;

public class ChatEntity : IBaseEntity
{
	public Guid Id { get; set; } = Guid.NewGuid();

	public string Name { get; set; }
	
	public string Title { get; set; }

	public ChatType Type { get; set; }
	
	public Guid? OwnerId { get; set; }
	
	public UserEntity Owner { get; set; }

	public string AvatarFileName { get; set; }
	
	public Guid? LastMessageId { get; set; }
	
	public MessageEntity LastMessage { get; set; }
	
	public List<ChatUserEntity> ChatUsers { get; set; } = new();
	
	public List<MessageEntity> Messages { get; set; } = new();

	public List<DeletedDialogByUserEntity> DeletedDialogByUsers { get; set; } = new();

	public List<BanUserByChatEntity> BanUserByChats { get; set; } = new();
	
	public ChatEntity(string name, string title, ChatType type, Guid? ownerId, string avatarFileName, Guid? lastMessageId)
	{
		Name = name;
		Title = title;
		Type = type;
		OwnerId = ownerId;
		AvatarFileName = avatarFileName;
		LastMessageId = lastMessageId;
		
		new ChatEntityValidator().ValidateAndThrow(this);
	}

	public void UpdateName(string name)
	{
		Name = name;
		new ChatEntityValidator().ValidateAndThrow(this);
	}
	
	public void UpdateTitle(string title)
	{
		Title = title;
		new ChatEntityValidator().ValidateAndThrow(this);
	}
	
	public void UpdateAvatarFileName(string avatarFileName)
	{
		AvatarFileName = avatarFileName;
		new ChatEntityValidator().ValidateAndThrow(this);
	}

	public void UpdateLastMessageId(Guid? lastMessageId)
	{
		LastMessageId = lastMessageId;
		new ChatEntityValidator().ValidateAndThrow(this);
	}
}