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
		
		var createConversationCommand = new CreateChatCommand(
			RequesterId: user21Th.Value.Id,
			Name: "qwerty",
			Title: "qwerty",
			Type: ChatType.Conversation,
			AvatarFile: null);

		var conversation = await MessengerModule.RequestAsync(createConversationCommand, CancellationToken.None);
		
		var joinToConversationCommand = new JoinToChatCommand(
			RequesterId: alice.Value.Id,
			ChatId: conversation.Value.Id);

		await MessengerModule.RequestAsync(joinToConversationCommand, CancellationToken.None);

		var createRoleUserAliceInConversationBy21ThCommand = new CreateOrUpdateRoleUserInConversationCommand(
			RequesterId: user21Th.Value.Id,
			ChatId: conversation.Value.Id,
			UserId: alice.Value.Id,
			RoleTitle: "moderator",
			RoleColor: RoleColor.Cyan,
			CanBanUser: true,
			CanChangeChatData: false,
			CanAddAndRemoveUserToConversation: false,
			CanGivePermissionToUser:false);

		await MessengerModule.RequestAsync(createRoleUserAliceInConversationBy21ThCommand, CancellationToken.None);

		var removeRoleUserInConversationCommand = new RemoveRoleUserInConversationCommand(
			RequesterId: user21Th.Value.Id,
			ChatId: conversation.Value.Id,
			UserId: alice.Value.Id);

		var removeRoleUserInConversationResult = await MessengerModule.RequestAsync(removeRoleUserInConversationCommand, CancellationToken.None);

		removeRoleUserInConversationResult.IsSuccess.Should().BeTrue();
	}
}