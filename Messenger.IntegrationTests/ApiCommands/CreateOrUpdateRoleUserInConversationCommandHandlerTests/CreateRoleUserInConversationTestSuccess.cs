using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.Domain.Enums;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.CreateOrUpdateRoleUserInConversationCommandHandlerTests;

public class CreateRoleUserInConversationTestSuccess : IntegrationTestBase, IIntegrationTest
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

		var createAliceRoleInConversationBy21ThResult =
			await RequestAsync(createAliceRoleInConversationBy21ThCommand, CancellationToken.None);

		createAliceRoleInConversationBy21ThResult.Value.RoleColor
			.Should().Be(createAliceRoleInConversationBy21ThCommand.RoleColor);
		
		createAliceRoleInConversationBy21ThResult.Value.RoleTitle
			.Should().Be(createAliceRoleInConversationBy21ThCommand.RoleTitle);
		
		createAliceRoleInConversationBy21ThResult.Value.CanBanUser
			.Should().Be(createAliceRoleInConversationBy21ThCommand.CanBanUser);
		
		createAliceRoleInConversationBy21ThResult.Value.CanChangeChatData
			.Should().Be(createAliceRoleInConversationBy21ThCommand.CanChangeChatData);
		
		createAliceRoleInConversationBy21ThResult.Value.CanGivePermissionToUser
			.Should().Be(createAliceRoleInConversationBy21ThCommand.CanGivePermissionToUser);
		
		createAliceRoleInConversationBy21ThResult.Value.CanAddAndRemoveUserToConversation
			.Should().Be(createAliceRoleInConversationBy21ThCommand.CanAddAndRemoveUserToConversation);
		
		createAliceRoleInConversationBy21ThResult.Value.IsOwner.Should().BeFalse();
	}
}