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
			user21Th.Value.Id,
			Name: "qwerty",
			Title: "qwerty",
			ChatType.Conversation,
			AvatarFile: null);

		var createConversationResult = await MessengerModule.RequestAsync(createConversationCommand, CancellationToken.None);
		
		var aliceJoinConversationCommand = new JoinToChatCommand(alice.Value.Id, createConversationResult.Value.Id);

		await MessengerModule.RequestAsync(aliceJoinConversationCommand, CancellationToken.None);

		var createAliceRoleInConversationBy21ThCommand = new CreateOrUpdateRoleUserInConversationCommand(
			user21Th.Value.Id,
			createConversationResult.Value.Id,
			alice.Value.Id,
			RoleTitle: "moderator",
			RoleColor.Cyan,
			CanBanUser: true,
			CanChangeChatData: false,
			CanAddAndRemoveUserToConversation: false,
			CanGivePermissionToUser:false);

		await MessengerModule.RequestAsync(createAliceRoleInConversationBy21ThCommand, CancellationToken.None);

		var removeAliceRoleInConversationCommand = new RemoveRoleUserInConversationCommand(
			user21Th.Value.Id,
			createConversationResult.Value.Id,
			alice.Value.Id);

		var removeAliceRoleInConversationResult = await MessengerModule.RequestAsync(removeAliceRoleInConversationCommand, CancellationToken.None);

		removeAliceRoleInConversationResult.IsSuccess.Should().BeTrue();
	}
}