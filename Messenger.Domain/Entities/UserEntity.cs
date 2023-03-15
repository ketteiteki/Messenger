using FluentValidation;
using Messenger.Domain.Entities.Abstraction;
using Messenger.Domain.Entities.Validation;

namespace Messenger.Domain.Entities;

public class UserEntity : IBaseEntity
{
	public Guid Id { get; set; } = Guid.NewGuid();
	
	public string DisplayName { get; set; }

	public string Nickname { get; set; }
	
	public string? Bio { get; set; }

	public string? AvatarLink { get; set; }
	
	public string PasswordHash { get; set; }

	public string PasswordSalt { get; set; }

	public List<MessageEntity> Messages { get; set; } = new();

	public List<ChatEntity> Chats { get; set; } = new();
	
	public List<ChatUserEntity> ChatUsers { get; set; } = new();
	
	public List<DeletedMessageByUserEntity> DeletedMessageByUsers { get; set; } = new();

	public List<DeletedDialogByUserEntity> DeletedDialogByUsers { get; set; } = new();
	
	public List<BanUserByChatEntity> BanUserByChats { get; set; } = new();

	public List<SessionEntity> Sessions { get; set; } = new();

	public UserEntity(string displayName, string nickname, string? bio, string? avatarLink, string passwordHash, string passwordSalt)
	{
		DisplayName = displayName;
		Nickname = nickname;
		Bio = bio;
		AvatarLink = avatarLink;
		PasswordHash = passwordHash;
		PasswordSalt = passwordSalt;
		
		new UserEntityValidator().ValidateAndThrow(this);
	}
}