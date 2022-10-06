using FluentAssertions;
using Messenger.BusinessLogic.Conversations.Commands;
using Messenger.Domain.Entities;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Messenger.IntegrationTests.Conversations.CommandsTests;

public class RemoveRoleUserInConversationCommandHandlerTestSuccess : IntegrationTestBase, IIntegrationTest
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
			canChangeChatData: false,
			canGivePermissionToUser: false,
			canAddAndRemoveUserToConversation: false,
			isOwner: false);

		DatabaseContextFixture.RoleUserByChats.Add(roleForAlice);
		await DatabaseContextFixture.SaveChangesAsync();

		var removeRoleUserInConversationCommand = new RemoveRoleUserInConversationCommand
		{
			RequesterId = user21th.Id,
			ChatId = conversation.Id,
			UserId = alice.Id
		};

		await MessengerModule.RequestAsync(removeRoleUserInConversationCommand, CancellationToken.None);

		var roleOfAlise = await DatabaseContextFixture.RoleUserByChats
			.FirstOrDefaultAsync(r => r.UserId == alice.Id && r.ChatId == conversation.Id);

		roleOfAlise.Should().BeNull();
	}
}