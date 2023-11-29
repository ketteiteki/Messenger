using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.Domain.Enums;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.UpdateConversationAvatarCommandHandlerTests;

public class UpdateConversationAvatarTestSuccess : IntegrationTestBase, IIntegrationTest
{
	[Fact]
    public async Task Test()
    {
        var user21Th = await RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
		var alice = await RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);

		var createConversationCommand = new CreateChatCommand(
			user21Th.Value.Id,
			Name: "qwerty",
			Title: "qwerty",
			ChatType.Conversation,
			AvatarFile: null);

		var createConversationResult = await RequestAsync(createConversationCommand, CancellationToken.None);

		var aliceJoinConversationCommand = new JoinToChatCommand(alice.Value.Id, createConversationResult.Value.Id);
		
		await RequestAsync(aliceJoinConversationCommand, CancellationToken.None);
		
		var createRoleAliceCommand = new CreateOrUpdateRoleUserInConversationCommand(
			user21Th.Value.Id,
			createConversationResult.Value.Id,
			alice.Value.Id,
			RoleTitle: "moderator",
			RoleColor.Blue,
			CanBanUser: false,
			CanChangeChatData: true,
			CanAddAndRemoveUserToConversation: false,
			CanGivePermissionToUser: false);

		await RequestAsync(createRoleAliceCommand, CancellationToken.None);

		var updateAvatarConversationBy21ThCommand = new UpdateChatAvatarCommand(
			user21Th.Value.Id,
			createConversationResult.Value.Id,
			FilesHelper.GetFile());
		
		var updateAvatarConversationByAliceCommand =new UpdateChatAvatarCommand(
			alice.Value.Id,
			createConversationResult.Value.Id,
			FilesHelper.GetFile());

		var updateAvatarConversationBy21ThResult = 
			await RequestAsync(updateAvatarConversationBy21ThCommand, CancellationToken.None);

		var updateAvatarConversationByAliceResult = 
			await RequestAsync(updateAvatarConversationByAliceCommand, CancellationToken.None);

		updateAvatarConversationBy21ThResult.Value.AvatarLink.Should().NotBeNull();
		updateAvatarConversationByAliceResult.Value.AvatarLink.Should().NotBeNull();

		var avatarFileName = updateAvatarConversationByAliceResult.Value.AvatarLink.Split("/")[^1];

		await BlobService.DeleteBlobAsync(avatarFileName);
    }
}