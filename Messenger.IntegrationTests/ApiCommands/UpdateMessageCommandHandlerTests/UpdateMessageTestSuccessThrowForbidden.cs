using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Messages;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Enum;
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
            RequesterId: user21Th.Value.Id,
            Name: "qwerty",
            Title: "qwerty",
            Type: ChatType.Conversation,
            AvatarFile: null);

        var conversation = await MessengerModule.RequestAsync(createConversationCommand, CancellationToken.None);
        
        await MessengerModule.RequestAsync(new JoinToChatCommand(
            RequesterId: bob.Value.Id,
            ChatId: conversation.Value.Id), CancellationToken.None);
        
        var createdMessageBy21ThResult = await MessengerModule.RequestAsync(new CreateMessageCommand(
            RequesterId: user21Th.Value.Id,
            Text: "qwerty2",
            ReplyToId: null,
            ChatId: conversation.Value.Id,
            Files: null), CancellationToken.None);

        var updateMessage21ThByBobCommand = new UpdateMessageCommand(
            RequesterId: bob.Value.Id,
            MessageId: createdMessageBy21ThResult.Value.Id,
            Text: "hello bro23");
        
        var updateMessageByBobResult = 
            await MessengerModule.RequestAsync(updateMessage21ThByBobCommand, CancellationToken.None);
        
        updateMessageByBobResult.Error.Should().BeOfType<ForbiddenError>();
    }
}