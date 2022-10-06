using FluentAssertions;
using Messenger.BusinessLogic.Conversations.Commands;
using Messenger.Domain.Entities;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.Conversations.CommandsTests;

public class UpdateConversationCommandHandlerTestSuccess : IntegrationTestBase, IIntegrationTest
{
	[Fact]
	public async Task Test()
	{
		var user21th = EntityHelper.CreateUser21th();
		var alice = EntityHelper.CreateUserAlice();

		DatabaseContextFixture.Users.AddRange(user21th, alice);
		await DatabaseContextFixture.SaveChangesAsync();

		var conversation = EntityHelper.CreateConversation(user21th.Id, "qwerty", "qwerty");
	
		DatabaseContextFixture.Chats.Add(conversation);
		await DatabaseContextFixture.SaveChangesAsync();

		DatabaseContextFixture.ChatUsers.AddRange(
			new ChatUser {UserId = user21th.Id, ChatId = conversation.Id},
			new ChatUser {UserId = alice.Id, ChatId = conversation.Id});
		await DatabaseContextFixture.SaveChangesAsync();
		
		var roleForAlice = new RoleUserByChat(
			userId: alice.Id,
			chatId: conversation.Id,
			roleTitle: "moderator",
			roleColor: RoleColor.Blue,
			canBanUser: false,
			canChangeChatData: true,
			canGivePermissionToUser: false,
			canAddAndRemoveUserToConversation: false,
			isOwner: false);

		DatabaseContextFixture.RoleUserByChats.Add(roleForAlice);
		await DatabaseContextFixture.SaveChangesAsync();

		var updateConversationCommandBy21th = new UpdateConversationCommand
		{
			RequesterId = user21th.Id,
			ChatId = conversation.Id,
			Name = "21thName",
			Title = "21thTitle"
		};
		
		var updateConversationCommandByAlice = new UpdateConversationCommand
		{
			RequesterId = alice.Id,
			ChatId = conversation.Id,
			Name = "AliceName",
			Title = "AliceTitle"
		};

		var conversationAfterUpdateBy21th = await MessengerModule.RequestAsync(updateConversationCommandBy21th, CancellationToken.None);

		conversationAfterUpdateBy21th.Name.Should().Be(updateConversationCommandBy21th.Name);
		conversationAfterUpdateBy21th.Title.Should().Be(updateConversationCommandBy21th.Title);
		
		var conversationAfterUpdateByAlice = await MessengerModule.RequestAsync(updateConversationCommandByAlice, CancellationToken.None);
		
		conversationAfterUpdateByAlice.Name.Should().Be(updateConversationCommandByAlice.Name);
		conversationAfterUpdateByAlice.Title.Should().Be(updateConversationCommandByAlice.Title);
	}
}