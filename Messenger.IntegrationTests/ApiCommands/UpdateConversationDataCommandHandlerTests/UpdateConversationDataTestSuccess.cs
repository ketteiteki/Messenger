using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.UpdateConversationDataCommandHandlerTests;

public class UpdateConversationDataTestSuccess : IntegrationTestBase, IIntegrationTest
{
	[Fact]
	public async Task Test()
	{
		var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
		var alice = await MessengerModule.RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);

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
		
		var updateConversationBy21ThCommand = new UpdateConversationCommand(
			RequesterId: user21Th.Value.Id,
			ChatId: conversation.Value.Id,
			Name: "21thName",
			Title: "21thTitle");
		
		var updateConversationByAliceCommand =new UpdateConversationCommand(
			RequesterId: alice.Value.Id,
			ChatId: conversation.Value.Id,
			Name: "AliceName",
			Title: "AliceTitle");

		var updateConversationBy21ThResult = await MessengerModule.RequestAsync(updateConversationBy21ThCommand,
			CancellationToken.None);

		updateConversationBy21ThResult.Value.Name.Should().Be(updateConversationBy21ThCommand.Name);
		updateConversationBy21ThResult.Value.Title.Should().Be(updateConversationBy21ThCommand.Title);
		
		var updateConversationByAliceResult = await MessengerModule.RequestAsync(updateConversationByAliceCommand, 
			CancellationToken.None);
		
		updateConversationByAliceResult.Value.Name.Should().Be(updateConversationByAliceCommand.Name);
		updateConversationByAliceResult.Value.Title.Should().Be(updateConversationByAliceCommand.Title);
	}
}