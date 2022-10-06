using FluentAssertions;
using Messenger.BusinessLogic.Conversations.Commands;
using Messenger.Domain.Entities;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Messenger.IntegrationTests.Conversations.CommandsTests;

public class BanUserInConversationCommandHandlerTessSuccess : IntegrationTestBase, IIntegrationTest
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
			canBanUser: true,
			canChangeChatData: false,
			canGivePermissionToUser: false,
			canAddAndRemoveUserToConversation: false,
			isOwner: false);
		
		DatabaseContextFixture.RoleUserByChats.Add(roleForBob);
		await DatabaseContextFixture.SaveChangesAsync();
			
		var banUserInConversationCommandForAlice = new BanUserInConversationCommand
		{
			RequesterId = user21th.Id,
			ChatId = conversation.Id,
			UserId = alice.Id,
			BanDateOfExpire = DateTime.UtcNow.AddDays(2)
		};
		
		var banUserInConversationCommandForAlex = new BanUserInConversationCommand
		{
			RequesterId = bob.Id,
			ChatId = conversation.Id,
			UserId = alex.Id,
			BanDateOfExpire = DateTime.UtcNow.AddDays(2)
		};

		await MessengerModule.RequestAsync(banUserInConversationCommandForAlice, CancellationToken.None);
		await MessengerModule.RequestAsync(banUserInConversationCommandForAlex, CancellationToken.None);

		var banOfAlice = await DatabaseContextFixture.BanUserByChats
			.FirstOrDefaultAsync(b => b.UserId == alice.Id && b.ChatId == conversation.Id);
		
		var banOfAlex = await DatabaseContextFixture.BanUserByChats
			.FirstOrDefaultAsync(b => b.UserId == alex.Id && b.ChatId == conversation.Id);

		banOfAlice.Should().NotBeNull();
		banOfAlex.Should().NotBeNull();
	}
}