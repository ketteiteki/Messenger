using Messenger.Domain.Entities;
using Messenger.Domain.Enum;

namespace Messenger.IntegrationTests.Helpers;

public class EntityHelper
{
	//create user
	public static User CreateUser21th()
	{
		return new User(
			displayName: "21th",
			nickName: "ketteiteki",
			bio: "qwerty",
			avatarLink: null,
			passwordHash: "ruweudyggdfu",
			passwordSalt: "fgtrhwjmvcx");
	}
	
	public static User CreateUserAlice()
	{
		return new User(
			displayName: "Alice",
			nickName: "Alice",
			bio: "qwerty",
			avatarLink: null,
			passwordHash: "weudydsggdfu",
			passwordSalt: "fgtrhwjmvcxgfkl");
	}
	
	public static User CreateUserBob()
	{
		return new User(
			displayName: "Bob",
			nickName: "Bob123",
			bio: "qwerty",
			avatarLink: null,
			passwordHash: "kgjfdlgd",
			passwordSalt: "rwievcj");
	}

	public static User CreateUserAlex()
	{
		return new User(
			displayName: "Alex",
			nickName: "Alex123",
			bio: "qwerty",
			avatarLink: null,
			passwordHash: "kg432jfdlgd",
			passwordSalt: "rwfnbvievcj");
	}
	//create chat
	public static Chat CreateDialog()
	{
		return new Chat(
			name: null,
			title: null,
			type: ChatType.Dialog,
			ownerId: null,
			avatarLink: null,
			lastMessageId: null);
	}
	
	public static Chat CreateChannel(Guid ownerId, string name, string title)
	{
		return new Chat(
			name: name,
			title: title,
			type: ChatType.Channel,
			ownerId: ownerId,
			avatarLink: null,
			lastMessageId: null);
	}
	
	public static Chat CreateConversation(Guid ownerId, string name, string title)
	{
		return new Chat(
			name: name,
			title: title,
			type: ChatType.Сonversation,
			ownerId: ownerId,
			avatarLink: null,
			lastMessageId: null);
	}
}