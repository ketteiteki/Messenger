using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Messages;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Enums;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.DeleteMessageCommandHandlerTests;

public class DeleteMessageTestThrowForbidden : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
        var alice = await MessengerModule.RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);
        var bob = await MessengerModule.RequestAsync(CommandHelper.RegistrationBobCommand(), CancellationToken.None);

        var createConversationCommand = new CreateChatCommand(
            user21Th.Value.Id,
            Name: "qwerty",
            Title: "qwerty",
            ChatType.Conversation,
            AvatarFile: null);

        var createConversationResult = await MessengerModule.RequestAsync(createConversationCommand, CancellationToken.None);

        var aliceJoinToConversationCommand = new JoinToChatCommand(alice.Value.Id, createConversationResult.Value.Id); 
        var bobJoinToConversationCommand = new JoinToChatCommand(bob.Value.Id, createConversationResult.Value.Id); 
        
        await MessengerModule.RequestAsync(aliceJoinToConversationCommand, CancellationToken.None);
        await MessengerModule.RequestAsync(bobJoinToConversationCommand, CancellationToken.None);

        var createMessageByAliceCommand = new CreateMessageCommand(
            alice.Value.Id,
            Text: "qwerty2",
            ReplyToId: null,
            createConversationResult.Value.Id,
            Files: null);
        
        var createMessageByAliceResult = 
            await MessengerModule.RequestAsync(createMessageByAliceCommand, CancellationToken.None);

        var deleteAliceMessageByBobCommand = new DeleteMessageCommand(
            bob.Value.Id,
            createMessageByAliceResult.Value.Id,
            IsDeleteForAll: true);
        
        var deleteAliceMessageByBobResult = 
            await MessengerModule.RequestAsync(deleteAliceMessageByBobCommand, CancellationToken.None);
        
        deleteAliceMessageByBobResult.Error.Should().BeOfType<ForbiddenError>();
    }
}