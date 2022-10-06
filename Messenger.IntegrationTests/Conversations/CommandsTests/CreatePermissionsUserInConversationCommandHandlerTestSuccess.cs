using FluentAssertions;
using Messenger.BusinessLogic.Conversations.Commands;
using Messenger.Domain.Entities;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.Conversations.CommandsTests;

public class CreatePermissionsUserInConversationCommandHandlerTestSuccess : IntegrationTestBase, IIntegrationTest
{
	[Fact]
	public async Task Test()
	{
		var user21th = EntityHelper.CreateUser21th();
		var alice = EntityHelper.CreateUserAlice();
		var bob = EntityHelper.CreateUserBob();
		var alex = EntityHelper.CreateUserAlex();

		DatabaseContextFixture.Users.AddRange(user21th, alice, bob, alex);
		await DatabaseContextFixture.SaveChangesAsync();

		var conversation = EntityHelper.CreateConversation(user21th.Id, "qwerty", "qwerty");
	
		DatabaseContextFixture.Chats.Add(conversation);
		await DatabaseContextFixture.SaveChangesAsync();

		DatabaseContextFixture.ChatUsers.AddRange(
			new ChatUser {UserId = user21th.Id, ChatId = conversation.Id},
			new ChatUser {UserId = alice.Id, ChatId = conversation.Id},
			new ChatUser {UserId = bob.Id, ChatId = conversation.Id},
			new ChatUser {UserId = alex.Id, ChatId = conversation.Id});
		await DatabaseContextFixture.SaveChangesAsync();
		
		var roleForAlice = new RoleUserByChat(
			userId: alice.Id,
			chatId: conversation.Id,
			roleTitle: "moderator",
			roleColor: RoleColor.Blue,
			canBanUser: false,
			canChangeChatData: false,
			canGivePermissionToUser: true,
			canAddAndRemoveUserToConversation: false,
			isOwner: false);

		DatabaseContextFixture.RoleUserByChats.Add(roleForAlice);
		await DatabaseContextFixture.SaveChangesAsync();

		var createPermissionsUserInConversationCommandBy21th = new CreatePermissionsUserInConversationCommand
		{
			RequesterId = user21th.Id,
			UserId = bob.Id,
			ChatId = conversation.Id,
			CanSendMedia = false,
		};

		var result21th = await MessengerModule.RequestAsync(createPermissionsUserInConversationCommandBy21th, CancellationToken.None);

		result21th.CanSendMedia.Should().BeFalse();
		
		var createPermissionsUserInConversationCommandByAlice = new CreatePermissionsUserInConversationCommand
		{
			RequesterId = alice.Id,
			UserId = alex.Id,
			ChatId = conversation.Id,
			CanSendMedia = false,
		};
		
		var resultAlice = await MessengerModule.RequestAsync(createPermissionsUserInConversationCommandByAlice, CancellationToken.None);

		resultAlice.CanSendMedia.Should().BeFalse();
	}
}