using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.Domain.Enums;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.CreateOrUpdateRoleUserInConversationCommandHandlerTests;

public class UpdateRoleUserInConversationTestSuccess : IntegrationTestBase, IIntegrationTest
{
	[Fact]
    public async Task Test()
    {
        var user21Th = await RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
		var alice = await RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);

		var createConversationCommand = new CreateChatCommand(
			user21Th.Value.Id,
			Name: "qwerty",
			Title: "qwerty",
			ChatType.Conversation,
			AvatarFile: null);
		
		var createConversationResult = await RequestAsync(createConversationCommand, CancellationToken.None);

		var addAliceInConversationBy21ThCommand = new AddUserToConversationCommand(
			user21Th.Value.Id,
			createConversationResult.Value.Id,
			alice.Value.Id);

		await RequestAsync(addAliceInConversationBy21ThCommand, CancellationToken.None);
		
		var createAliceRoleInConversationBy21ThCommand = new CreateOrUpdateRoleUserInConversationCommand(
			user21Th.Value.Id,
			createConversationResult.Value.Id,
			alice.Value.Id,
			RoleTitle: "moderator",
			RoleColor.Cyan,
			CanBanUser: true,
			CanChangeChatData: false,
			CanAddAndRemoveUserToConversation: true,
			CanGivePermissionToUser: false);

	    await RequestAsync(createAliceRoleInConversationBy21ThCommand, CancellationToken.None);

		var updateAliceRoleInConversationBy21ThCommand = new CreateOrUpdateRoleUserInConversationCommand(
			user21Th.Value.Id,
			createConversationResult.Value.Id,
			alice.Value.Id,
			RoleTitle: "moderator",
			RoleColor.Cyan,
			CanBanUser: false,
			CanChangeChatData: true,
			CanAddAndRemoveUserToConversation: true,
			CanGivePermissionToUser: true);

		var updateAliceRoleInConversationBy21ThResult =
			await RequestAsync(updateAliceRoleInConversationBy21ThCommand, CancellationToken.None);
		
		updateAliceRoleInConversationBy21ThResult.Value.RoleColor
			.Should().Be(updateAliceRoleInConversationBy21ThCommand.RoleColor);
		
		updateAliceRoleInConversationBy21ThResult.Value.RoleTitle
			.Should().Be(updateAliceRoleInConversationBy21ThCommand.RoleTitle);
		
		updateAliceRoleInConversationBy21ThResult.Value.CanBanUser
			.Should().Be(updateAliceRoleInConversationBy21ThCommand.CanBanUser);
		
		updateAliceRoleInConversationBy21ThResult.Value.CanChangeChatData
			.Should().Be(updateAliceRoleInConversationBy21ThCommand.CanChangeChatData);
		
		updateAliceRoleInConversationBy21ThResult.Value.CanGivePermissionToUser
			.Should().Be(updateAliceRoleInConversationBy21ThCommand.CanGivePermissionToUser);

		updateAliceRoleInConversationBy21ThResult.Value.CanAddAndRemoveUserToConversation
			.Should().Be(updateAliceRoleInConversationBy21ThCommand.CanAddAndRemoveUserToConversation);
			
		updateAliceRoleInConversationBy21ThResult.Value.IsOwner.Should().BeFalse();
    }
}