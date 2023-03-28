using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.Domain.Enums;
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
			user21Th.Value.Id,
			Name: "qwerty",
			Title: "qwerty",
			ChatType.Conversation,
			AvatarFile: null);
		
		var createConversationResult = await MessengerModule.RequestAsync(createConversationCommand, CancellationToken.None);

		var userAliceJoinToConversationCommand = new JoinToChatCommand(alice.Value.Id, createConversationResult.Value.Id);
		var userBobJoinToConversationCommand = new JoinToChatCommand(bob.Value.Id, createConversationResult.Value.Id);
		var userAlexJoinToConversationCommand = new JoinToChatCommand(alex.Value.Id, createConversationResult.Value.Id);
		
		await MessengerModule.RequestAsync(userAliceJoinToConversationCommand, CancellationToken.None);
		await MessengerModule.RequestAsync(userBobJoinToConversationCommand, CancellationToken.None);
		await MessengerModule.RequestAsync(userAlexJoinToConversationCommand, CancellationToken.None);

		var createBobRoleInConversationBy21ThCommand = new CreateOrUpdateRoleUserInConversationCommand(
			user21Th.Value.Id,
			createConversationResult.Value.Id,
			bob.Value.Id,
			RoleTitle: "moderator",
			RoleColor.Black,
			CanBanUser: true,
			CanChangeChatData: false,
			CanAddAndRemoveUserToConversation: false,
			CanGivePermissionToUser: false);
		
		await MessengerModule.RequestAsync(createBobRoleInConversationBy21ThCommand, CancellationToken.None);

		var banUserAliceInConversationBy21ThCommand = new BanUserInConversationCommand(
			RequesterId: user21Th.Value.Id,
			ChatId: createConversationResult.Value.Id,
			UserId: alice.Value.Id,
			BanMinutes: 15);

		var banUserAlexInConversationCommandByBobCommand = new BanUserInConversationCommand(
			RequesterId: bob.Value.Id,
			ChatId: createConversationResult.Value.Id,
			UserId: alex.Value.Id,
			BanMinutes: 15);

		await MessengerModule.RequestAsync(banUserAliceInConversationBy21ThCommand, CancellationToken.None);
		await MessengerModule.RequestAsync(banUserAlexInConversationCommandByBobCommand, CancellationToken.None);

		var secondUserAliceJoinToConversationCommand =
			new JoinToChatCommand(alice.Value.Id, createConversationResult.Value.Id);
		var secondUserAlexJoinToConversationCommand =
			new JoinToChatCommand(alex.Value.Id, createConversationResult.Value.Id);

		var secondUserAliceJoinToConversationResult =
			await MessengerModule.RequestAsync(secondUserAliceJoinToConversationCommand, CancellationToken.None);
		var secondUserAlexJoinToConversationResult =
			await MessengerModule.RequestAsync(secondUserAlexJoinToConversationCommand, CancellationToken.None);

		secondUserAliceJoinToConversationResult.IsSuccess.Should().BeFalse();
		secondUserAlexJoinToConversationResult.IsSuccess.Should().BeFalse();
	}
}