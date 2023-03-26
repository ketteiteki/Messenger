using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Messages;
using Messenger.Domain.Enums;
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
            user21Th.Value.Id,
            Name: "qwerty",
            Title: "qwerty",
            ChatType.Conversation,
            AvatarFile: null);
		
        var createConversationResult = await MessengerModule.RequestAsync(createConversationCommand, CancellationToken.None);

        var userAliceJoinToConversationCommand =
            new JoinToChatCommand(alice.Value.Id, createConversationResult.Value.Id);
        
        await MessengerModule.RequestAsync(userAliceJoinToConversationCommand, CancellationToken.None);

        var createMessageBy21ThCommand = new CreateMessageCommand(
            user21Th.Value.Id,
            Text: "qwerty1",
            ReplyToId: null,
            createConversationResult.Value.Id,
            Files: null);

        var createMessageBy21ThResult = await MessengerModule.RequestAsync(createMessageBy21ThCommand, CancellationToken.None);
        
        var createMessageByAliceCommand = new CreateMessageCommand(
            alice.Value.Id,
            Text: "qwerty2",
            createMessageBy21ThResult.Value.Id,
            createConversationResult.Value.Id,
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