using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.CreateOrUpdateRoleUserInConversationCommandHandlerTests;

public class CreateOrUpdateRoleUserInConversationTestSuccess : IntegrationTestBase, IIntegrationTest
{
	[Fact]
	public async Task Test()
	{
		var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
		var alice = await MessengerModule.RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);

		var createConversationCommand = new CreateConversationCommand(
			RequesterId: user21Th.Value.Id,
			Name: "qwerty",
			Title: "qwerty",
			AvatarFile: null);

		var conversation = await MessengerModule.RequestAsync(createConversationCommand, CancellationToken.None);

		var addAliceInConversationCommand = new AddUserToConversationCommand(
			RequesterId: user21Th.Value.Id,
			ChatId: conversation.Value.Id,
			UserId: alice.Value.Id);

		await MessengerModule.RequestAsync(addAliceInConversationCommand, CancellationToken.None);
		
		var createOrUpdateRoleUserInConversation = new CreateOrUpdateRoleUserInConversationCommand(
			RequesterId: user21Th.Value.Id,
			ChatId: conversation.Value.Id,
			UserId: alice.Value.Id,
			RoleTitle: "moderator",
			RoleColor: RoleColor.Cyan,
			CanBanUser: true,
			CanChangeChatData: false,
			CanAddAndRemoveUserToConversation: true,
			CanGivePermissionToUser:false);

		var role = await MessengerModule.RequestAsync(createOrUpdateRoleUserInConversation, CancellationToken.None);

		role.Value.RoleColor.Should().Be(createOrUpdateRoleUserInConversation.RoleColor);
		role.Value.RoleTitle.Should().Be(createOrUpdateRoleUserInConversation.RoleTitle);
		role.Value.CanBanUser.Should().Be(createOrUpdateRoleUserInConversation.CanBanUser);
		role.Value.CanChangeChatData.Should().Be(createOrUpdateRoleUserInConversation.CanChangeChatData);
		role.Value.CanGivePermissionToUser.Should().Be(createOrUpdateRoleUserInConversation.CanGivePermissionToUser);
		role.Value.CanAddAndRemoveUserToConversation.Should().Be(createOrUpdateRoleUserInConversation.CanAddAndRemoveUserToConversation);
		role.Value.IsOwner.Should().Be(false);
	}
}