using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Messages;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Enums;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.UpdateMessageCommandHandlerTests;

public class UpdateMessageTestSuccessThrowForbidden : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
        var bob = await MessengerModule.RequestAsync(CommandHelper.RegistrationBobCommand(), CancellationToken.None);
        
        var createConversationCommand = new CreateChatCommand(
            user21Th.Value.Id,
            Name: "qwerty",
            Title: "qwerty",
            ChatType.Conversation,
            AvatarFile: null);

        var createConversationResult = await MessengerModule.RequestAsync(createConversationCommand, CancellationToken.None);

        var bobJoinChatCommand = new JoinToChatCommand(bob.Value.Id, createConversationResult.Value.Id);
        
        await MessengerModule.RequestAsync(bobJoinChatCommand, CancellationToken.None);

        var createMessageBy21ThCommand = new CreateMessageCommand(
            user21Th.Value.Id,
            Text: "qwerty2",
            ReplyToId: null,
            createConversationResult.Value.Id,
            Files: null);
        
        var createdMessageBy21ThResult = await MessengerModule.RequestAsync(createMessageBy21ThCommand, CancellationToken.None);

        var updateMessage21ThByBobCommand = new UpdateMessageCommand(
            bob.Value.Id,
            createdMessageBy21ThResult.Value.Id,
            Text: "hello bro23");
        
        var updateMessageByBobResult = 
            await MessengerModule.RequestAsync(updateMessage21ThByBobCommand, CancellationToken.None);
        
        updateMessageByBobResult.Error.Should().BeOfType<ForbiddenError>();
    }
}