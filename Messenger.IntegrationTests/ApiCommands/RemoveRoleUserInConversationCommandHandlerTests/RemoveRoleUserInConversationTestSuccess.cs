using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.RemoveRoleUserInConversationCommandHandlerTests;

public class RemoveRoleUserInConversationTestSuccess : IntegrationTestBase, IIntegrationTest
{
	[Fact]
	public async Task Test()
	{
		var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
		var alice = await MessengerModule.RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);
		
		var command = new CreateConversationCommand(
			RequesterId: user21Th.Value.Id,
			Name: "qwerty",
			Title: "qwerty",
			AvatarFile: null);

		var conversation = await MessengerModule.RequestAsync(command, CancellationToken.None);
		
		var joinToConversationCommand = new JoinToChatCommand(
			RequesterId: alice.Value.Id,
			ChatId: conversation.Value.Id);

		await MessengerModule.RequestAsync(joinToConversationCommand, CancellationToken.None);

		var createOrUpdateRoleUserInConversation = new CreateOrUpdateRoleUserInConversationCommand(
			RequesterId: user21Th.Value.Id,
			ChatId: conversation.Value.Id,
			UserId: alice.Value.Id,
			RoleTitle: "moderator",
			RoleColor: RoleColor.Cyan,
			CanBanUser: true,
			CanChangeChatData: false,
			CanAddAndRemoveUserToConversation: false,
			CanGivePermissionToUser:false);

		await MessengerModule.RequestAsync(createOrUpdateRoleUserInConversation, CancellationToken.None);

		var removeRoleUserInConversationCommand = new RemoveRoleUserInConversationCommand(
			RequesterId: user21Th.Value.Id,
			ChatId: conversation.Value.Id,
			UserId: alice.Value.Id);

		var role = await MessengerModule.RequestAsync(removeRoleUserInConversationCommand, CancellationToken.None);

		role.IsSuccess.Should().BeTrue();
	}
}