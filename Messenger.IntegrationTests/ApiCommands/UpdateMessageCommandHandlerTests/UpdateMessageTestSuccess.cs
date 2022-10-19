using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.BusinessLogic.ApiCommands.Messages;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.UpdateMessageCommandHandlerTests;

public class UpdateMessageTestSuccess : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
        var alice = await MessengerModule.RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);
        var bob = await MessengerModule.RequestAsync(CommandHelper.RegistrationBobCommand(), CancellationToken.None);
        
        var conversation = await MessengerModule.RequestAsync(new CreateConversationCommand(
            RequesterId: user21Th.Value.Id,
            Name: "qwerty",
            Title: "qwerty",
            AvatarFile: null), CancellationToken.None);

        await MessengerModule.RequestAsync(new JoinToChatCommand(
            RequesterId: alice.Value.Id,
            ChatId: conversation.Value.Id), CancellationToken.None);
        
        var createdMessageBy21Th = await MessengerModule.RequestAsync(new CreateMessageCommand(
            RequesterId: user21Th.Value.Id,
            Text: "qwerty2",
            ReplyToId: null,
            ChatId: conversation.Value.Id,
            Files: null), CancellationToken.None);

        var updateMessageBy21ThCommand = new UpdateMessageCommand(
            RequesterId: user21Th.Value.Id,
            MessageId: createdMessageBy21Th.Value.Id,
            Text: "hello bro");
        
        var updateMessageByAliceCommand = new UpdateMessageCommand(
            RequesterId: alice.Value.Id,
            MessageId: createdMessageBy21Th.Value.Id,
            Text: "hello bro2232");
        
        var updateMessageByBobCommand = new UpdateMessageCommand(
            RequesterId: bob.Value.Id,
            MessageId: createdMessageBy21Th.Value.Id,
            Text: "hello bro23");
        
        var updatedMessageBy21Th = await MessengerModule.RequestAsync(updateMessageBy21ThCommand, CancellationToken.None);
        
        var updatedMessageByAlice = await MessengerModule.RequestAsync(updateMessageByAliceCommand, CancellationToken.None);

        var updatedMessageByBob = await MessengerModule.RequestAsync(updateMessageByBobCommand, CancellationToken.None);
        
        updatedMessageBy21Th.Value.Text.Should().Be(updateMessageBy21ThCommand.Text);

        updatedMessageByAlice.IsSuccess.Should().BeFalse();

        updatedMessageByBob.IsSuccess.Should().BeFalse();
    }
}