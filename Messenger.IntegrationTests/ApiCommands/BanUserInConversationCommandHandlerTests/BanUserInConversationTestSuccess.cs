using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.BanUserInConversationCommandHandlerTests;

public class BanUserInConversationTestSuccess : IntegrationTestBase, IIntegrationTest
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

		await MessengerModule.RequestAsync(
			new JoinToChatCommand(
				RequesterId: alice.Value.Id,
				ChatId: conversation.Value.Id), CancellationToken.None);
		await MessengerModule.RequestAsync(
			new JoinToChatCommand(
					RequesterId: bob.Value.Id,
					ChatId: conversation.Value.Id), CancellationToken.None);
		await MessengerModule.RequestAsync(
			new JoinToChatCommand(
				RequesterId: alex.Value.Id,
				ChatId: conversation.Value.Id), CancellationToken.None);

		var createRoleBobInConversationBy21ThCommand = new CreateOrUpdateRoleUserInConversationCommand(
			RequesterId: user21Th.Value.Id,
			UserId: bob.Value.Id,
			ChatId: conversation.Value.Id,
			RoleColor: RoleColor.Black,
			RoleTitle: "moderator",
			CanBanUser: true,
			CanChangeChatData: false,
			CanGivePermissionToUser: false,
			CanAddAndRemoveUserToConversation: false);
		
		await MessengerModule.RequestAsync(createRoleBobInConversationBy21ThCommand, CancellationToken.None);

		var banUserAliceInConversationBy21ThCommand = new BanUserInConversationCommand(
			RequesterId: user21Th.Value.Id,
			ChatId: conversation.Value.Id,
			UserId: alice.Value.Id,
			BanMinutes: 15);

		var banUserAlexInConversationCommandByBobCommand = new BanUserInConversationCommand(
			RequesterId: bob.Value.Id,
			ChatId: conversation.Value.Id,
			UserId: alex.Value.Id,
			BanMinutes: 15);

		await MessengerModule.RequestAsync(banUserAliceInConversationBy21ThCommand, CancellationToken.None);
		await MessengerModule.RequestAsync(banUserAlexInConversationCommandByBobCommand, CancellationToken.None);

		var joinInChatByAliceResult = await MessengerModule.RequestAsync(
			new JoinToChatCommand(
				RequesterId: alice.Value.Id,
				ChatId: conversation.Value.Id), CancellationToken.None);
		
		var joinInChatByAlexCommand = await MessengerModule.RequestAsync(
			new JoinToChatCommand(
				RequesterId: alex.Value.Id, 
				ChatId: conversation.Value.Id), CancellationToken.None);

		joinInChatByAliceResult.IsSuccess.Should().BeFalse();
		
		joinInChatByAlexCommand.IsSuccess.Should().BeFalse();
	}
}