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

        var createMessageBy21ThResult = 
            await MessengerModule.RequestAsync(createMessageBy21ThCommand, CancellationToken.None);
        
        var createFirstMessageByAliceCommand = new CreateMessageCommand(
            alice.Value.Id,
            Text: "qwerty2",
            createMessageBy21ThResult.Value.Id,
            createConversationResult.Value.Id,
            Files: null);

        var createFirstMessageByAliceResult = 
            await MessengerModule.RequestAsync(createFirstMessageByAliceCommand , CancellationToken.None);
        
        var createSecondMessageByAliceCommand = new CreateMessageCommand(
            alice.Value.Id,
            Text: "qwerty2",
            createMessageBy21ThResult.Value.Id,
            createConversationResult.Value.Id,
            Files: null);

        var createSecondMessageByAliceResult = 
            await MessengerModule.RequestAsync(createSecondMessageByAliceCommand , CancellationToken.None);
        
        createMessageBy21ThResult.Value.Text.Should().Be(createMessageBy21ThCommand.Text);
        createMessageBy21ThResult.Value.ChatId.Should().Be(createMessageBy21ThCommand.ChatId);
        createMessageBy21ThResult.Value.OwnerDisplayName.Should().Be(user21Th.Value.DisplayName);
        createMessageBy21ThResult.Value.OwnerAvatarLink.Should().Be(user21Th.Value.AvatarLink);
        
        createFirstMessageByAliceResult.Value.Text.Should().Be(createSecondMessageByAliceCommand.Text);
        createFirstMessageByAliceResult.Value.ChatId.Should().Be(createSecondMessageByAliceCommand.ChatId);
        createFirstMessageByAliceResult.Value.OwnerDisplayName.Should().Be(alice.Value.DisplayName);
        createFirstMessageByAliceResult.Value.OwnerAvatarLink.Should().Be(alice.Value.AvatarLink);
        createFirstMessageByAliceResult.Value.ReplyToMessageId.Should().Be(createMessageBy21ThResult.Value.Id);
        createFirstMessageByAliceResult.Value.ReplyToMessageText.Should().Be(createMessageBy21ThResult.Value.Text);
        createFirstMessageByAliceResult.Value.ReplyToMessageAuthorDisplayName.Should().Be(user21Th.Value.DisplayName);
        
        createSecondMessageByAliceResult.Value.Text.Should().Be(createSecondMessageByAliceCommand.Text);
        createSecondMessageByAliceResult.Value.ChatId.Should().Be(createSecondMessageByAliceCommand.ChatId);
        createSecondMessageByAliceResult.Value.OwnerDisplayName.Should().Be(alice.Value.DisplayName);
        createSecondMessageByAliceResult.Value.OwnerAvatarLink.Should().Be(alice.Value.AvatarLink);
        createSecondMessageByAliceResult.Value.ReplyToMessageId.Should().Be(createMessageBy21ThResult.Value.Id);
        createSecondMessageByAliceResult.Value.ReplyToMessageText.Should().Be(createMessageBy21ThResult.Value.Text);
        createSecondMessageByAliceResult.Value.ReplyToMessageAuthorDisplayName.Should().Be(user21Th.Value.DisplayName);
    }
}