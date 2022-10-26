using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.CreateOrUpdateRoleUserInConversationCommandHandlerTests;

public class UpdateRoleUserInConversationTestSuccess : IntegrationTestBase, IIntegrationTest
{
	[Fact]
    public async Task Test()
    {
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
		var alice = await MessengerModule.RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);

		var createConversationCommand = new CreateChatCommand(
			RequesterId: user21Th.Value.Id,
			Name: "qwerty",
			Title: "qwerty",
			Type: ChatType.Conversation,
			AvatarFile: null);
		
		var conversation = await MessengerModule.RequestAsync(createConversationCommand, CancellationToken.None);

		var addAliceInConversationBy21ThCommand = new AddUserToConversationCommand(
			RequesterId: user21Th.Value.Id,
			ChatId: conversation.Value.Id,
			UserId: alice.Value.Id);

		await MessengerModule.RequestAsync(addAliceInConversationBy21ThCommand, CancellationToken.None);
		
		var createRoleUserAliceInConversationBy21ThCommand = new CreateOrUpdateRoleUserInConversationCommand(
			RequesterId: user21Th.Value.Id,
			ChatId: conversation.Value.Id,
			UserId: alice.Value.Id,
			RoleTitle: "moderator",
			RoleColor: RoleColor.Cyan,
			CanBanUser: true,
			CanChangeChatData: false,
			CanAddAndRemoveUserToConversation: true,
			CanGivePermissionToUser: false);

	    await MessengerModule.RequestAsync(createRoleUserAliceInConversationBy21ThCommand, CancellationToken.None);

		var updateRoleUserAliceInConversationBy21ThCommand = new CreateOrUpdateRoleUserInConversationCommand(
			RequesterId: user21Th.Value.Id,
			ChatId: conversation.Value.Id,
			UserId: alice.Value.Id,
			RoleTitle: "moderator",
			RoleColor: RoleColor.Cyan,
			CanBanUser: false,
			CanChangeChatData: true,
			CanAddAndRemoveUserToConversation: true,
			CanGivePermissionToUser: true);

		var updateRoleUserAliceInConversationBy21ThResult =
			await MessengerModule.RequestAsync(updateRoleUserAliceInConversationBy21ThCommand, CancellationToken.None);
		
		updateRoleUserAliceInConversationBy21ThResult.Value.RoleColor
			.Should().Be(updateRoleUserAliceInConversationBy21ThCommand.RoleColor);
		updateRoleUserAliceInConversationBy21ThResult.Value.RoleTitle
			.Should().Be(updateRoleUserAliceInConversationBy21ThCommand.RoleTitle);
		updateRoleUserAliceInConversationBy21ThResult.Value.CanBanUser
			.Should().Be(updateRoleUserAliceInConversationBy21ThCommand.CanBanUser);
		updateRoleUserAliceInConversationBy21ThResult.Value.CanChangeChatData
			.Should().Be(updateRoleUserAliceInConversationBy21ThCommand.CanChangeChatData);
		updateRoleUserAliceInConversationBy21ThResult.Value.CanGivePermissionToUser
			.Should().Be(updateRoleUserAliceInConversationBy21ThCommand.CanGivePermissionToUser);
		updateRoleUserAliceInConversationBy21ThResult.Value.CanAddAndRemoveUserToConversation
			.Should().Be(updateRoleUserAliceInConversationBy21ThCommand.CanAddAndRemoveUserToConversation);
		updateRoleUserAliceInConversationBy21ThResult.Value.IsOwner.Should().BeFalse();
    }
}