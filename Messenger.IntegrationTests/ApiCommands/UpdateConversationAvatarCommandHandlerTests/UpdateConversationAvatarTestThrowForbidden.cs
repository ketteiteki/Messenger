using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Enums;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.UpdateConversationAvatarCommandHandlerTests;

public class UpdateConversationAvatarTestThrowForbidden : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
		var alice = await RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);

		var createConversationCommand = new CreateChatCommand(
			RequesterId: user21Th.Value.Id,
			Name: "qwerty",
			Title: "qwerty",
			Type: ChatType.Conversation,
			AvatarFile: null);

		var conversation = await RequestAsync(createConversationCommand, CancellationToken.None);
		
		await RequestAsync(
			new JoinToChatCommand(
			RequesterId: alice.Value.Id,
			ChatId: conversation.Value.Id), CancellationToken.None);
		
		var updateAvatarConversationByAliceCommand =new UpdateChatAvatarCommand(
			RequesterId: alice.Value.Id,
			ChatId: conversation.Value.Id,
			AvatarFile: FilesHelper.GetFile());

		var updateAvatarConversationByAliceResult = 
			await RequestAsync(updateAvatarConversationByAliceCommand, CancellationToken.None);

		updateAvatarConversationByAliceResult.Error.Should().BeOfType<ForbiddenError>();
    }
}