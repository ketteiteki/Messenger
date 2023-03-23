using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.UpdateConversationAvatarCommandHandlerTests;

public class UpdateConversationAvatarTestThrowForbidden : IntegrationTestBase, IIntegrationTest
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
		
		var updateAvatarConversationByAliceCommand =new UpdateChatAvatarCommand(
			RequesterId: alice.Value.Id,
			ChatId: conversation.Value.Id,
			AvatarFile: FilesHelper.GetFile());

		var updateAvatarConversationByAliceResult = 
			await MessengerModule.RequestAsync(updateAvatarConversationByAliceCommand, CancellationToken.None);

		updateAvatarConversationByAliceResult.Error.Should().BeOfType<ForbiddenError>();
    }
}