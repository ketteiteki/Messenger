using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.UpdateConversationAvatarCommandHandlerTests;

public class UpdateConversationAvatarTestSuccess : IntegrationTestBase, IIntegrationTest
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

		var updateAvatarConversationBy21ThCommand = new UpdateChatAvatarCommand(
			RequesterId: user21Th.Value.Id,
			ChatId: conversation.Value.Id,
			AvatarFile: FilesHelper.GetFile());
		
		var updateAvatarConversationByAliceCommand =new UpdateChatAvatarCommand(
			RequesterId: alice.Value.Id,
			ChatId: conversation.Value.Id,
			AvatarFile: FilesHelper.GetFile());

		var updateAvatarConversationBy21ThResult = 
			await MessengerModule.RequestAsync(updateAvatarConversationBy21ThCommand, CancellationToken.None);

		updateAvatarConversationBy21ThResult.Value.AvatarLink.Should().NotBeNull();
		
		var updateAvatarConversationByAliceResult = 
			await MessengerModule.RequestAsync(updateAvatarConversationByAliceCommand, CancellationToken.None);
		
		updateAvatarConversationByAliceResult.Value.AvatarLink.Should().NotBeNull();
    }
}