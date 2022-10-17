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
		if (user21Th == null) throw new ArgumentNullException(nameof(user21Th));
		var alice = await MessengerModule.RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);
		var bob = await MessengerModule.RequestAsync(CommandHelper.RegistrationBobCommand(), CancellationToken.None);
		var alex = await MessengerModule.RequestAsync(CommandHelper.RegistrationAlexCommand(), CancellationToken.None);

		var createConversationCommand = new CreateConversationCommand(
			RequesterId: user21Th.Value.Id,
			Name: "qwerty",
			Title: "qwerty",
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

		var createRoleBobInConversation = new CreateOrUpdateRoleUserInConversationCommand(
			RequesterId: user21Th.Value.Id,
			UserId: bob.Value.Id,
			ChatId: conversation.Value.Id,
			RoleColor: RoleColor.Black,
			RoleTitle: "moderator",
			CanBanUser: true,
			CanChangeChatData: false,
			CanGivePermissionToUser: false,
			CanAddAndRemoveUserToConversation: false);
		
		await MessengerModule.RequestAsync(createRoleBobInConversation, CancellationToken.None);

		var banUserInConversationCommandForAliceBy21Th = new BanUserInConversationCommand(
			RequesterId: user21Th.Value.Id,
			ChatId: conversation.Value.Id,
			UserId: alice.Value.Id,
			BanDateOfExpire: DateTime.UtcNow.AddDays(2));

		var banUserInConversationCommandForAlexByBob = new BanUserInConversationCommand(
			RequesterId: bob.Value.Id,
			ChatId: conversation.Value.Id,
			UserId: alex.Value.Id,
			BanDateOfExpire: DateTime.UtcNow.AddDays(2));

		await MessengerModule.RequestAsync(banUserInConversationCommandForAliceBy21Th, CancellationToken.None);
		await MessengerModule.RequestAsync(banUserInConversationCommandForAlexByBob, CancellationToken.None);

		var joinInChatByAlice = await MessengerModule.RequestAsync(
			new JoinToChatCommand(
				RequesterId: alice.Value.Id,
				ChatId: conversation.Value.Id), CancellationToken.None);
		
		var joinInChatByAlex = await MessengerModule.RequestAsync(
			new JoinToChatCommand(
				RequesterId: alex.Value.Id, 
				ChatId: conversation.Value.Id), CancellationToken.None);

		joinInChatByAlice.IsSuccess.Should().BeFalse();
		
		joinInChatByAlex.IsSuccess.Should().BeFalse();
	}
}