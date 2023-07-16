using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Messages;
using Messenger.Domain.Enums;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.UpdateMessageCommandHandlerTests;

public class UpdateMessageTestSuccess : IntegrationTestBase, IIntegrationTest
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

        var createMessageBy21ThCommand = new CreateMessageCommand(
            user21Th.Value.Id,
            Text: "qwerty2",
            ReplyToId: null,
            createConversationResult.Value.Id,
            Files: null);
        
        var createdMessageBy21ThResult = await RequestAsync(createMessageBy21ThCommand, CancellationToken.None);

        var updateMessageBy21ThCommand = new UpdateMessageCommand(
            user21Th.Value.Id,
            createdMessageBy21ThResult.Value.Id,
            Text: "hello bro");
        
        var updateMessageByAliceCommand = new UpdateMessageCommand(
            alice.Value.Id,
            createdMessageBy21ThResult.Value.Id,
            Text: "hello bro2232");
        
        var updateMessageBy21ThResult =
            await RequestAsync(updateMessageBy21ThCommand, CancellationToken.None);
        
        var updateMessageByAliceResult =
            await RequestAsync(updateMessageByAliceCommand, CancellationToken.None);

        updateMessageBy21ThResult.Value.Text.Should().Be(updateMessageBy21ThCommand.Text);
        updateMessageByAliceResult.IsSuccess.Should().BeFalse();
    }
}