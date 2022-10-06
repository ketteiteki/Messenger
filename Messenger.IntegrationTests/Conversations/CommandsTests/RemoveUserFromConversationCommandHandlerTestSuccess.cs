using FluentAssertions;
using Messenger.BusinessLogic.Conversations.Commands;
using Messenger.Domain.Entities;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Messenger.IntegrationTests.Conversations.CommandsTests;

public class RemoveUserFromConversationCommandHandlerTestSuccess : IntegrationTestBase, IIntegrationTest
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

		var roleForBob = new RoleUserByChat(
			userId: bob.Id,
			chatId: conversation.Id,
			roleTitle: "moderator",
			roleColor: RoleColor.Black,
			canBanUser: false,
			canChangeChatData: false,
			canGivePermissionToUser: false,
			canAddAndRemoveUserToConversation: true,
			isOwner: false);
		
		DatabaseContextFixture.RoleUserByChats.Add(roleForBob);
		await DatabaseContextFixture.SaveChangesAsync();
			
		var removeUserFromConversationCommandForAlice = new RemoveUserFromConversationCommand
		{
			RequesterId = user21th.Id,
			ChatId = conversation.Id,
			UserId = alice.Id,
		};
		
		var removeUserFromConversationCommandForAlex = new RemoveUserFromConversationCommand
		{
			RequesterId = bob.Id,
			ChatId = conversation.Id,
			UserId = alex.Id,
		};

		await MessengerModule.RequestAsync(removeUserFromConversationCommandForAlice, CancellationToken.None);
		await MessengerModule.RequestAsync(removeUserFromConversationCommandForAlex, CancellationToken.None);

		var chatUserOfAlice = await DatabaseContextFixture.ChatUsers
			.FirstOrDefaultAsync(b => b.UserId == alice.Id && b.ChatId == conversation.Id);
		
		var chatUserOfAlex = await DatabaseContextFixture.ChatUsers
			.FirstOrDefaultAsync(b => b.UserId == alex.Id && b.ChatId == conversation.Id);

		chatUserOfAlice.Should().BeNull();
		chatUserOfAlex.Should().BeNull();
	}
}