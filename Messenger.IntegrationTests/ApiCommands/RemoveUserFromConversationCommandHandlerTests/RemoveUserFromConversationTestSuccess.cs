using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.RemoveUserFromConversationCommandHandlerTests;

public class RemoveUserFromConversationTestSuccess : IntegrationTestBase, IIntegrationTest
{
	[Fact]
	public async Task Test()
	{
		var user21th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
		var alice = await MessengerModule.RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);
		var bob = await MessengerModule.RequestAsync(CommandHelper.RegistrationBobCommand(), CancellationToken.None);
		var alex = await MessengerModule.RequestAsync(CommandHelper.RegistrationAlexCommand(), CancellationToken.None);

		var createConversationCommand = new CreateConversationCommand(
			RequesterId: user21th.Value.Id,
			Name: "qwerty",
			Title: "qwerty",
			AvatarFile: null);

		var conversation = await MessengerModule.RequestAsync(createConversationCommand, CancellationToken.None);

		var addAliceInConversationCommand = new AddUserToConversationCommand(
			RequesterId: user21th.Value.Id,
			ChatId: conversation.Value.Id,
			UserId: alice.Value.Id);
		
		var addBobInConversationCommand = new AddUserToConversationCommand(
			RequesterId: user21th.Value.Id,
			ChatId: conversation.Value.Id,
			UserId: bob.Value.Id);
		
		var addAlexInConversationCommand = new AddUserToConversationCommand(
			RequesterId: user21th.Value.Id,
			ChatId: conversation.Value.Id,
			UserId: alex.Value.Id);

		await MessengerModule.RequestAsync(addAliceInConversationCommand, CancellationToken.None);
		await MessengerModule.RequestAsync(addBobInConversationCommand, CancellationToken.None);
		await MessengerModule.RequestAsync(addAlexInConversationCommand, CancellationToken.None);

		var createRoleForBobCommand = new CreateOrUpdateRoleUserInConversationCommand(
			RequesterId: user21th.Value.Id,
			UserId: bob.Value.Id,
			ChatId: conversation.Value.Id,
			RoleTitle: "moderator",
			RoleColor: RoleColor.Blue,
			CanBanUser: false,
			CanChangeChatData: false,
			CanGivePermissionToUser: false,
			CanAddAndRemoveUserToConversation: true);

		await MessengerModule.RequestAsync(createRoleForBobCommand, CancellationToken.None);

		var removeUserFromConversationCommandForAlice = new RemoveUserFromConversationCommand(
			RequesterId: user21th.Value.Id,
			ChatId: conversation.Value.Id,
			UserId: alice.Value.Id);
		
		var removeUserFromConversationCommandForAlex = new RemoveUserFromConversationCommand(
			RequesterId: bob.Value.Id,
			ChatId: conversation.Value.Id,
			UserId: alex.Value.Id);

		var removedAlice = await MessengerModule.RequestAsync(removeUserFromConversationCommandForAlice, CancellationToken.None);
		var removedAlex = await MessengerModule.RequestAsync(removeUserFromConversationCommandForAlex, CancellationToken.None);

		removedAlice.IsSuccess.Should().BeTrue();
		removedAlex.IsSuccess.Should().BeTrue();
	}
}