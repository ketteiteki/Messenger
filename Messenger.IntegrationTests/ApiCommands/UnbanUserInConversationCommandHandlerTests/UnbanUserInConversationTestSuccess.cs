using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.UnbanUserInConversationCommandHandlerTests;

public class UnbanUserInConversationTestSuccess : IntegrationTestBase, IIntegrationTest
{
	[Fact]
	public async Task Test()
	{
		var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
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
		
		var banForAliceCommand = new BanUserInConversationCommand(
			RequesterId: user21Th.Value.Id,
			ChatId: conversation.Value.Id,
			UserId: alice.Value.Id,
			BanDateOfExpire: DateTime.UtcNow.AddDays(2));
		
		var banForAlexCommand = new BanUserInConversationCommand(
			RequesterId: user21Th.Value.Id,
			ChatId: conversation.Value.Id,
			UserId: alex.Value.Id,
			BanDateOfExpire: DateTime.UtcNow.AddDays(2));

		await MessengerModule.RequestAsync(banForAliceCommand, CancellationToken.None);
		await MessengerModule.RequestAsync(banForAlexCommand, CancellationToken.None);
		
		var createRoleBobInConversation = new CreateOrUpdateRoleUserInConversationCommand(
			RequesterId: user21Th.Value.Id,
			ChatId: conversation.Value.Id,
			UserId: bob.Value.Id,
			RoleTitle: "moderator",
			RoleColor: RoleColor.Cyan,
			CanBanUser: true,
			CanChangeChatData: false,
			CanAddAndRemoveUserToConversation: true,
			CanGivePermissionToUser:false);

		await MessengerModule.RequestAsync(createRoleBobInConversation, CancellationToken.None);

		var unbanUserInConversationCommandForAliceCommand = new UnbanUserInConversationCommand(
			RequesterId: user21Th.Value.Id,
			ChatId: conversation.Value.Id,
			UserId: alice.Value.Id);
		
		var unbanUserInConversationCommandForAlexCommand = new UnbanUserInConversationCommand(
			RequesterId: bob.Value.Id,
			ChatId: conversation.Value.Id,
			UserId: alex.Value.Id);

		var unbanAlice = await MessengerModule.RequestAsync(unbanUserInConversationCommandForAliceCommand, CancellationToken.None);
		var unbanAlex = await MessengerModule.RequestAsync(unbanUserInConversationCommandForAlexCommand, CancellationToken.None);

		unbanAlice.IsSuccess.Should().BeTrue();
		unbanAlex.IsSuccess.Should().BeTrue();
	}
}