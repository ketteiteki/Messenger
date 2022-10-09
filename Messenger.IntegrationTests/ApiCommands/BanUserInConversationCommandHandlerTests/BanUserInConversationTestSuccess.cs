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
		var user21th = await MessengerModule.RequestAsync(CommandHelper.Registration21thCommand(), CancellationToken.None);
		var alice = await MessengerModule.RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);
		var bob = await MessengerModule.RequestAsync(CommandHelper.RegistrationBobCommand(), CancellationToken.None);
		var alex = await MessengerModule.RequestAsync(CommandHelper.RegistrationAlexCommand(), CancellationToken.None);

		var createConversationCommand = CommandHelper.CreateConversationCommand(user21th.Id, "qwerty", "qwerty", null);

		var conversation = await MessengerModule.RequestAsync(createConversationCommand, CancellationToken.None);

		await MessengerModule.RequestAsync(
			new JoinToConversationCommand(
				RequestorId: alice.Id,
				ChatId: conversation.Id), CancellationToken.None);
		await MessengerModule.RequestAsync(
			new JoinToConversationCommand(
					RequestorId: bob.Id,
					ChatId: conversation.Id), CancellationToken.None);
		await MessengerModule.RequestAsync(
			new JoinToConversationCommand(
				RequestorId: alex.Id,
				ChatId: conversation.Id), CancellationToken.None);

		var createRoleBobInConversation = new CreateOrUpdateRoleUserInConversationCommand(
			RequestorId: user21th.Id,
			UserId: bob.Id,
			ChatId: conversation.Id,
			RoleColor: RoleColor.Black,
			RoleTitle: "moderator",
			CanBanUser: true,
			CanChangeChatData: false,
			CanGivePermissionToUser: false,
			CanAddAndRemoveUserToConversation: false);
		
		await MessengerModule.RequestAsync(createRoleBobInConversation, CancellationToken.None);

		var banUserInConversationCommandForAliceBy21th = new BanUserInConversationCommand(
			RequestorId: user21th.Id,
			ChatId: conversation.Id,
			UserId: alice.Id,
			BanDateOfExpire: DateTime.UtcNow.AddDays(2));

		var banUserInConversationCommandForAlexByBob = new BanUserInConversationCommand(
			RequestorId: bob.Id,
			ChatId: conversation.Id,
			UserId: alex.Id,
			BanDateOfExpire: DateTime.UtcNow.AddDays(2));

		await MessengerModule.RequestAsync(banUserInConversationCommandForAliceBy21th, CancellationToken.None);
		await MessengerModule.RequestAsync(banUserInConversationCommandForAlexByBob, CancellationToken.None);

		await MessengerModule.RequestAsync(
			new JoinToConversationCommand(
				RequestorId: alice.Id,
				ChatId: conversation.Id), CancellationToken.None);
		await MessengerModule.RequestAsync(
			new JoinToConversationCommand(
				RequestorId: alex.Id, 
				ChatId: conversation.Id), CancellationToken.None);
	}
}