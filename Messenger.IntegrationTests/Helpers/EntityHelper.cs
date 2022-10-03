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
	
	public static User CreateUserKhachatur()
	{
		return new User(
			displayName: "Khachatur",
			nickName: "Khachatur",
			bio: "qwerty",
			avatarLink: null,
			passwordHash: "weudydsggdfu",
			passwordSalt: "fgtrhwjmvcxgfkl");
	}
	//create chat
	public static Chat CreateChannel(Guid ownerId, string name, string title)
	{
		return new Chat(
			name: "convers",
			title: "21ths den",
			type: ChatType.Channel,
			ownerId: ownerId,
			avatarLink: null,
			lastMessageId: null);
	}
}