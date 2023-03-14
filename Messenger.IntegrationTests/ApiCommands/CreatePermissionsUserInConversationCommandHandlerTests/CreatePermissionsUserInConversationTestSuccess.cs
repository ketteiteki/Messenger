using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.CreatePermissionsUserInConversationCommandHandlerTests;

public class CreatePermissionsUserInConversationTestSuccess : IntegrationTestBase, IIntegrationTest
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

		var addAliceInConversationBy21ThCommand = new AddUserToConversationCommand(
			RequesterId: user21Th.Value.Id,
			ChatId: conversation.Value.Id,
			UserId: alice.Value.Id);
		
		var addBobInConversationBy21ThCommand = new AddUserToConversationCommand(
			RequesterId: user21Th.Value.Id,
			ChatId: conversation.Value.Id,
			UserId: bob.Value.Id);
		
		var addAlexInConversationBy21ThCommand = new AddUserToConversationCommand(
			RequesterId: user21Th.Value.Id,
			ChatId: conversation.Value.Id,
			UserId: alex.Value.Id);

		await MessengerModule.RequestAsync(addAliceInConversationBy21ThCommand, CancellationToken.None);
		await MessengerModule.RequestAsync(addBobInConversationBy21ThCommand, CancellationToken.None);
		await MessengerModule.RequestAsync(addAlexInConversationBy21ThCommand, CancellationToken.None);
		
		var createRoleForAliceCommand = new CreateOrUpdateRoleUserInConversationCommand(
			RequesterId: user21Th.Value.Id,
			UserId: alice.Value.Id,
			ChatId: conversation.Value.Id,
			RoleTitle: "moderator",
			RoleColor: RoleColor.Blue,
			CanBanUser: false,
			CanChangeChatData: false,
			CanGivePermissionToUser: true,
			CanAddAndRemoveUserToConversation: false);

		await MessengerModule.RequestAsync(createRoleForAliceCommand, CancellationToken.None);
		
		var createPermissionsUserInConversationBy21ThCommand = new CreatePermissionsUserInConversationCommand(
			RequesterId: user21Th.Value.Id,
			UserId: bob.Value.Id,
			ChatId: conversation.Value.Id,
			CanSendMedia: false,
			MuteMinutes: 5);

		var createPermissionsUserInConversationByAliceCommand = new CreatePermissionsUserInConversationCommand(
			RequesterId: alice.Value.Id,
			UserId: alex.Value.Id,
			ChatId: conversation.Value.Id,
			CanSendMedia: false,
			MuteMinutes: null);
		
		var createPermissionsUserBobInConversationByAlexCommand = new CreatePermissionsUserInConversationCommand(
			RequesterId: alex.Value.Id,
			UserId: bob.Value.Id,
			ChatId: conversation.Value.Id,
			CanSendMedia: false,
			MuteMinutes: 10);
		
		var createPermissionsUserInConversationBy21ThResult =
			await MessengerModule.RequestAsync(createPermissionsUserInConversationBy21ThCommand, CancellationToken.None);
		var createPermissionsUserInConversationByAliceResult = 
			await MessengerModule.RequestAsync(createPermissionsUserInConversationByAliceCommand, CancellationToken.None);
		var createPermissionsUserBobInConversationByAlexResult = 
			await MessengerModule.RequestAsync(createPermissionsUserBobInConversationByAlexCommand, CancellationToken.None);

		createPermissionsUserInConversationBy21ThResult.Value.CanSendMedia.Should().BeFalse();
		createPermissionsUserInConversationBy21ThResult.Value.MuteDateOfExpire.Should().NotBeNull();
		createPermissionsUserInConversationByAliceResult.Value.CanSendMedia.Should().BeFalse();
		createPermissionsUserBobInConversationByAlexResult.Error.Should().BeOfType<ForbiddenError>();
	}
}