using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Messages;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.CreateMessageCommandHandlerTests;

public class CreateMessageTestSuccess : IntegrationTestBase, IIntegrationTest
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

        await MessengerModule.RequestAsync(new JoinToChatCommand(
            RequesterId: alice.Value.Id,
            ChatId: conversation.Value.Id), CancellationToken.None);

        var createMessageBy21ThCommand = new CreateMessageCommand(
            RequesterId: user21Th.Value.Id,
            Text: "qwerty1",
            ReplyToId: null,
            ChatId: conversation.Value.Id,
            Files: null);

        var createMessageBy21ThResult = await MessengerModule.RequestAsync(createMessageBy21ThCommand, CancellationToken.None);
        
        var createMessageByAliceCommand = new CreateMessageCommand(
            RequesterId: alice.Value.Id,
            Text: "qwerty2",
            ReplyToId: createMessageBy21ThResult.Value.Id,
            ChatId: conversation.Value.Id,
            Files: null);

        var createMessageByAliceResult = await MessengerModule.RequestAsync(createMessageByAliceCommand , CancellationToken.None);
        
        createMessageBy21ThResult.Value.Text.Should().Be(createMessageBy21ThCommand.Text);
        createMessageBy21ThResult.Value.ChatId.Should().Be(createMessageBy21ThCommand.ChatId);
        createMessageBy21ThResult.Value.OwnerDisplayName.Should().Be(user21Th.Value.DisplayName);
        createMessageBy21ThResult.Value.OwnerAvatarLink.Should().Be(user21Th.Value.AvatarLink);
        
        createMessageByAliceResult.Value.Text.Should().Be(createMessageByAliceCommand.Text);
        createMessageByAliceResult.Value.ChatId.Should().Be(createMessageByAliceCommand.ChatId);
        createMessageByAliceResult.Value.OwnerDisplayName.Should().Be(alice.Value.DisplayName);
        createMessageByAliceResult.Value.OwnerAvatarLink.Should().Be(alice.Value.AvatarLink);
        createMessageByAliceResult.Value.ReplyToMessageId.Should().Be(createMessageBy21ThResult.Value.Id);
        createMessageByAliceResult.Value.ReplyToMessageText.Should().Be(createMessageBy21ThResult.Value.Text);
        createMessageByAliceResult.Value.ReplyToMessageAuthorDisplayName.Should().Be(user21Th.Value.DisplayName);
    }
}