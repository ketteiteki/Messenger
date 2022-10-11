using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.UpdateConversationCommandHandlerTests;

public class UpdateConversationTestSuccess : IntegrationTestBase, IIntegrationTest
{
	[Fact]
	public async Task Test()
	{
		var user21th = await MessengerModule.RequestAsync(CommandHelper.Registration21thCommand(), CancellationToken.None);
		var alice = await MessengerModule.RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);

		var createConversationCommand = new CreateConversationCommand(
			RequestorId: user21th.Value.Id,
			Name: "qwerty",
			Title: "qwerty",
			AvatarFile: null);

		var conversation = await MessengerModule.RequestAsync(createConversationCommand, CancellationToken.None);

		await MessengerModule.RequestAsync(
			new JoinToConversationCommand(
			RequestorId: alice.Value.Id,
			ChatId: conversation.Value.Id), CancellationToken.None);
		
		var createRoleAliceCommand = new CreateOrUpdateRoleUserInConversationCommand(
			RequestorId: user21th.Value.Id,
			UserId: alice.Value.Id,
			ChatId: conversation.Value.Id,
			RoleTitle: "moderator",
			RoleColor: RoleColor.Blue,
			CanBanUser: false,
			CanChangeChatData: true,
			CanGivePermissionToUser: false,
			CanAddAndRemoveUserToConversation: false);

		await MessengerModule.RequestAsync(createRoleAliceCommand, CancellationToken.None);
		
		var updateConversationCommandBy21th = new UpdateConversationCommand(
			RequestorId: user21th.Value.Id,
			ChatId: conversation.Value.Id,
			Name: "21thName",
			Title: "21thTitle");
		
		var updateConversationCommandByAlice =new UpdateConversationCommand(
			RequestorId: alice.Value.Id,
			ChatId: conversation.Value.Id,
			Name: "AliceName",
			Title: "AliceTitle");

		var conversationAfterUpdateBy21th = await MessengerModule.RequestAsync(updateConversationCommandBy21th,
			CancellationToken.None);

		conversationAfterUpdateBy21th.Value.Name.Should().Be(updateConversationCommandBy21th.Name);
		conversationAfterUpdateBy21th.Value.Title.Should().Be(updateConversationCommandBy21th.Title);
		
		var conversationAfterUpdateByAlice = await MessengerModule.RequestAsync(updateConversationCommandByAlice, 
			CancellationToken.None);
		
		conversationAfterUpdateByAlice.Value.Name.Should().Be(updateConversationCommandByAlice.Name);
		conversationAfterUpdateByAlice.Value.Title.Should().Be(updateConversationCommandByAlice.Title);
	}
}