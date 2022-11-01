using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.DeleteConversationCommandHandlerTests;

public class DeleteConversationTestThrowForbidden : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
        var alice = await MessengerModule.RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);
        var bob = await MessengerModule.RequestAsync(CommandHelper.RegistrationBobCommand(), CancellationToken.None);
        
        var createChannelCommand = new CreateChatCommand(
            RequesterId: user21Th.Value.Id,
            Name: "qwerty",
            Title: "qwerty",
            Type: ChatType.Conversation,
            AvatarFile: null);
		
        var channel = await MessengerModule.RequestAsync(createChannelCommand, CancellationToken.None);

        await MessengerModule.RequestAsync(new JoinToChatCommand(
            RequesterId: alice.Value.Id,
            ChatId: channel.Value.Id), CancellationToken.None);
        
        var deleteConversationByAliceCommand = new DeleteChatCommand(
            RequesterId: alice.Value.Id,
            ChatId: channel.Value.Id);
		
        var deleteConversationByAliceResult = 
            await MessengerModule.RequestAsync(deleteConversationByAliceCommand, CancellationToken.None);
        
        var deleteConversationByBobCommand = new DeleteChatCommand(
            RequesterId: bob.Value.Id,
            ChatId: channel.Value.Id);
		
        var deleteConversationByBobResult = 
            await MessengerModule.RequestAsync(deleteConversationByBobCommand, CancellationToken.None);

        deleteConversationByAliceResult.Error.Should().BeOfType<ForbiddenError>();
        deleteConversationByBobResult.Error.Should().BeOfType<ForbiddenError>();
    }
}