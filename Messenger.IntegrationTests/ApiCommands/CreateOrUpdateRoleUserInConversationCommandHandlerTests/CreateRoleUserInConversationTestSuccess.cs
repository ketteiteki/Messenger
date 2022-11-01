using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.CreateOrUpdateRoleUserInConversationCommandHandlerTests;

public class CreateRoleUserInConversationTestSuccess : IntegrationTestBase, IIntegrationTest
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

		var createRoleUserAliceInConversationBy21ThResult =
			await MessengerModule.RequestAsync(createRoleUserAliceInConversationBy21ThCommand, CancellationToken.None);

		createRoleUserAliceInConversationBy21ThResult.Value.RoleColor
			.Should().Be(createRoleUserAliceInConversationBy21ThCommand.RoleColor);
		createRoleUserAliceInConversationBy21ThResult.Value.RoleTitle
			.Should().Be(createRoleUserAliceInConversationBy21ThCommand.RoleTitle);
		createRoleUserAliceInConversationBy21ThResult.Value.CanBanUser
			.Should().Be(createRoleUserAliceInConversationBy21ThCommand.CanBanUser);
		createRoleUserAliceInConversationBy21ThResult.Value.CanChangeChatData
			.Should().Be(createRoleUserAliceInConversationBy21ThCommand.CanChangeChatData);
		createRoleUserAliceInConversationBy21ThResult.Value.CanGivePermissionToUser
			.Should().Be(createRoleUserAliceInConversationBy21ThCommand.CanGivePermissionToUser);
		createRoleUserAliceInConversationBy21ThResult.Value.CanAddAndRemoveUserToConversation
			.Should().Be(createRoleUserAliceInConversationBy21ThCommand.CanAddAndRemoveUserToConversation);
		createRoleUserAliceInConversationBy21ThResult.Value.IsOwner.Should().BeFalse();
	}
}