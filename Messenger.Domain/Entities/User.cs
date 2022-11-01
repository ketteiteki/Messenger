using FluentValidation;
using Messenger.Domain.Entities.Abstraction;
using Messenger.Domain.Entities.Validation;

namespace Messenger.Domain.Entities;

public class User : IBaseEntity
{
	public Guid Id { get; set; } = Guid.NewGuid();
	
	public string DisplayName { get; set; }

	public string Nickname { get; set; }
	
	public string? Bio { get; set; }

	public string? AvatarLink { get; set; }
	
	public string PasswordHash { get; set; }

	public string PasswordSalt { get; set; }

	public List<Message> Messages { get; set; } = new();

	public List<Chat> Chats { get; set; } = new();
	
	public List<ChatUser> ChatUsers { get; set; } = new();
	
	public List<DeletedMessageByUser> DeletedMessageByUsers { get; set; } = new();

	public List<DeletedDialogByUser> DeletedDialogByUsers { get; set; } = new();
	
	public List<BanUserByChat> BanUserByChats { get; set; } = new();

	public List<Session> Sessions { get; set; } = new();

	public User(string displayName, string nickname, string? bio, string? avatarLink, string passwordHash, string passwordSalt)
	{
		DisplayName = displayName;
		Nickname = nickname;
		Bio = bio;
		AvatarLink = avatarLink;
		PasswordHash = passwordHash;
		PasswordSalt = passwordSalt;
		
		new UserValidator().ValidateAndThrow(this);
	}
}