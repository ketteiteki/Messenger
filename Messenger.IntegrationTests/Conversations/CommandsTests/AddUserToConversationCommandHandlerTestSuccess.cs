using Messenger.BusinessLogic.Conversations.Commands;
using Messenger.Domain.Entities;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.Conversations.CommandsTests;

public class AddUserToConversationCommandHandlerTestSuccess : IntegrationTestBase, IIntegrationTest
{
	[Fact]
	public async Task Test()
	{
		var user21th = EntityHelper.CreateUser21th();
		var alice = EntityHelper.CreateUserAlice();
		var bob = EntityHelper.CreateUserBob();
		
		DatabaseContextFixture.Users.AddRange(user21th, alice, bob);
		await DatabaseContextFixture.SaveChangesAsync();

		var conversation = EntityHelper.CreateConversation(user21th.Id, "qwerty", "qwerty");
	
		DatabaseContextFixture.Chats.Add(conversation);
		await DatabaseContextFixture.SaveChangesAsync();

		DatabaseContextFixture.ChatUsers.AddRange(new ChatUser {UserId = user21th.Id, ChatId = conversation.Id});
		await DatabaseContextFixture.SaveChangesAsync();

		var AddUserToConversationBy21th = new AddUserToConversationCommand
		{
			RequestorId = user21th.Id,
			ChatId = conversation.Id,
			UserId = alice.Id
		};
		
		await MessengerModule.RequestAsync(AddUserToConversationBy21th, CancellationToken.None);

		DatabaseContextFixture.RoleUserByChats.Add(
			new RoleUserByChat(
				userId: alice.Id,
				chatId: conversation.Id,
				roleColor: RoleColor.Red,
				roleTitle: "moderator",
				canBanUser: false,
				canChangeChatData: false,
				canGivePermissionToUser: false,
				canAddAndRemoveUserToConversation: true,
				isOwner: false));
		await DatabaseContextFixture.SaveChangesAsync();
		
		var AddUserToConversationByAlice = new AddUserToConversationCommand
		{
			RequestorId = alice.Id,
			ChatId = conversation.Id,
			UserId = bob.Id
		};
		
		await MessengerModule.RequestAsync(AddUserToConversationByAlice, CancellationToken.None);
	}
}