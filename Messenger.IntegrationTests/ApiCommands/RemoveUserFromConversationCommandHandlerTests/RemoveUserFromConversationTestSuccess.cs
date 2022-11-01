using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
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
		var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
		var alice = await MessengerModule.RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);
		var bob = await MessengerModule.RequestAsync(CommandHelper.RegistrationBobCommand(), CancellationToken.None);
		var alex = await MessengerModule.RequestAsync(CommandHelper.RegistrationAlexCommand(), CancellationToken.None);

		var createConversationCommand = new CreateChatCommand(
			RequesterId: user21Th.Value.Id,
			Name: "qwerty",
			Title: "qwerty",
			Type: ChatType.Conversation,
			AvatarFile: null);

		var conversation = await MessengerModule.RequestAsync(createConversationCommand, CancellationToken.None);
		
		var addAliceInConversationCommand = new AddUserToConversationCommand(
			RequesterId: user21Th.Value.Id,
			ChatId: conversation.Value.Id,
			UserId: alice.Value.Id);
		
		var addBobInConversationCommand = new AddUserToConversationCommand(
			RequesterId: user21Th.Value.Id,
			ChatId: conversation.Value.Id,
			UserId: bob.Value.Id);
		
		var addAlexInConversationCommand = new AddUserToConversationCommand(
			RequesterId: user21Th.Value.Id,
			ChatId: conversation.Value.Id,
			UserId: alex.Value.Id);

		await MessengerModule.RequestAsync(addAliceInConversationCommand, CancellationToken.None);
		await MessengerModule.RequestAsync(addBobInConversationCommand, CancellationToken.None);
		await MessengerModule.RequestAsync(addAlexInConversationCommand, CancellationToken.None);

		var createRoleForBobCommand = new CreateOrUpdateRoleUserInConversationCommand(
			RequesterId: user21Th.Value.Id,
			UserId: bob.Value.Id,
			ChatId: conversation.Value.Id,
			RoleTitle: "moderator",
			RoleColor: RoleColor.Blue,
			CanBanUser: false,
			CanChangeChatData: false,
			CanGivePermissionToUser: false,
			CanAddAndRemoveUserToConversation: true);

		await MessengerModule.RequestAsync(createRoleForBobCommand, CancellationToken.None);

		var removeUserAliceFromConversationBy21ThCommand= new RemoveUserFromConversationCommand(
			RequesterId: user21Th.Value.Id,
			ChatId: conversation.Value.Id,
			UserId: alice.Value.Id);
		
		var removeUserAlexFromConversationByBobCommand = new RemoveUserFromConversationCommand(
			RequesterId: bob.Value.Id,
			ChatId: conversation.Value.Id,
			UserId: alex.Value.Id);

		var removeUserAliceFromConversationBy21ThResult =
			await MessengerModule.RequestAsync(removeUserAliceFromConversationBy21ThCommand, CancellationToken.None);
		var removeUserAlexFromConversationByBobResult =
			await MessengerModule.RequestAsync(removeUserAlexFromConversationByBobCommand, CancellationToken.None);

		removeUserAliceFromConversationBy21ThResult.IsSuccess.Should().BeTrue();
		removeUserAlexFromConversationByBobResult.IsSuccess.Should().BeTrue();
	}
}