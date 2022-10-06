using FluentAssertions;
using Messenger.BusinessLogic.Conversations.Commands;
using Messenger.Domain.Entities;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Messenger.IntegrationTests.Conversations.CommandsTests;

public class CreateOrUpdateRoleUserInConversationCommandHandlerTestSuccess : IntegrationTestBase, IIntegrationTest
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

		var CreateOrUpdateRoleUserInConversation = new CreateOrUpdateRoleUserInConversationCommand
		{
			RequesterId = user21th.Id,
			ChatId = conversation.Id,
			UserId = alice.Id,
			RoleTitle = "moderator",
			RoleColor = RoleColor.Cyan,
			CanBanUser = true,
			CanChangeChatData = false,
			CanAddAndRemoveUserToConversation = true,
			CanGivePermissionToUser = false
		};

		await MessengerModule.RequestAsync(CreateOrUpdateRoleUserInConversation, CancellationToken.None);

		var chatUser = await DatabaseContextFixture.ChatUsers
			.Include(c => c.Role)
			.FirstAsync(c => c.UserId == alice.Id && c.ChatId == conversation.Id);

		chatUser.Role?.RoleColor.Should().Be(CreateOrUpdateRoleUserInConversation.RoleColor);
		chatUser.Role?.RoleTitle.Should().Be(CreateOrUpdateRoleUserInConversation.RoleTitle);
		chatUser.Role?.CanBanUser.Should().Be(CreateOrUpdateRoleUserInConversation.CanBanUser);
		chatUser.Role?.CanChangeChatData.Should().Be(CreateOrUpdateRoleUserInConversation.CanChangeChatData);
		chatUser.Role?.CanGivePermissionToUser.Should().Be(CreateOrUpdateRoleUserInConversation.CanGivePermissionToUser);
		chatUser.Role?.CanAddAndRemoveUserToConversation.Should().Be(CreateOrUpdateRoleUserInConversation.CanAddAndRemoveUserToConversation);
		chatUser.Role?.IsOwner.Should().Be(false);
	}
}