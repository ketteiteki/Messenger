using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
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
		var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
		var alice = await MessengerModule.RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);

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
		
		var createRoleAliceCommand = new CreateOrUpdateRoleUserInConversationCommand(
			RequesterId: user21Th.Value.Id,
			UserId: alice.Value.Id,
			ChatId: conversation.Value.Id,
			RoleTitle: "moderator",
			RoleColor: RoleColor.Blue,
			CanBanUser: false,
			CanChangeChatData: true,
			CanGivePermissionToUser: false,
			CanAddAndRemoveUserToConversation: false);

		await MessengerModule.RequestAsync(createRoleAliceCommand, CancellationToken.None);
		
		var updateConversationCommandBy21Th = new UpdateConversationCommand(
			RequesterId: user21Th.Value.Id,
			ChatId: conversation.Value.Id,
			Name: "21thName",
			Title: "21thTitle");
		
		var updateConversationCommandByAlice =new UpdateConversationCommand(
			RequesterId: alice.Value.Id,
			ChatId: conversation.Value.Id,
			Name: "AliceName",
			Title: "AliceTitle");

		var conversationAfterUpdateBy21Th = await MessengerModule.RequestAsync(updateConversationCommandBy21Th,
			CancellationToken.None);

		conversationAfterUpdateBy21Th.Value.Name.Should().Be(updateConversationCommandBy21Th.Name);
		conversationAfterUpdateBy21Th.Value.Title.Should().Be(updateConversationCommandBy21Th.Title);
		
		var conversationAfterUpdateByAlice = await MessengerModule.RequestAsync(updateConversationCommandByAlice, 
			CancellationToken.None);
		
		conversationAfterUpdateByAlice.Value.Name.Should().Be(updateConversationCommandByAlice.Name);
		conversationAfterUpdateByAlice.Value.Title.Should().Be(updateConversationCommandByAlice.Title);
	}
}