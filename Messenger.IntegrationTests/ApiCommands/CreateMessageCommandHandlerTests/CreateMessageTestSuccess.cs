using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.BusinessLogic.ApiCommands.Messages;
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
        var bob = await MessengerModule.RequestAsync(CommandHelper.RegistrationBobCommand(), CancellationToken.None);

        var conversation = await MessengerModule.RequestAsync(new CreateConversationCommand(
            RequesterId: user21Th.Value.Id,
            Name: "qwerty",
            Title: "qwerty",
            AvatarFile: null), CancellationToken.None);

        await MessengerModule.RequestAsync(new JoinToChatCommand(
            RequesterId: alice.Value.Id,
            ChatId: conversation.Value.Id), CancellationToken.None);

        var createdMessageBy21ThCommand = new CreateMessageCommand(
            RequesterId: user21Th.Value.Id,
            Text: "qwerty1",
            ReplyToId: null,
            ChatId: conversation.Value.Id,
            Files: null);

        var createdMessageBy21Th = await MessengerModule.RequestAsync(createdMessageBy21ThCommand, CancellationToken.None);
        
        var createdMessageByAliceCommand = new CreateMessageCommand(
            RequesterId: alice.Value.Id,
            Text: "qwerty2",
            ReplyToId: createdMessageBy21Th.Value.Id,
            ChatId: conversation.Value.Id,
            Files: null);

        var createdMessageByBobCommand = new CreateMessageCommand(
            RequesterId: bob.Value.Id,
            Text: "qwerty3",
            ReplyToId: null,
            ChatId: conversation.Value.Id,
            Files: null);
        
        var createdMessageByAlice = await MessengerModule.RequestAsync(createdMessageByAliceCommand , CancellationToken.None);
        
        var createdMessageByBob = await MessengerModule.RequestAsync(createdMessageByBobCommand, CancellationToken.None);

        createdMessageBy21Th.Value.Text.Should().Be(createdMessageBy21ThCommand.Text);
        createdMessageBy21Th.Value.ChatId.Should().Be(createdMessageBy21ThCommand.ChatId);
        createdMessageBy21Th.Value.OwnerDisplayName.Should().Be(user21Th.Value.DisplayName);
        createdMessageBy21Th.Value.OwnerAvatarLink.Should().Be(user21Th.Value.AvatarLink);
        
        createdMessageByAlice.Value.Text.Should().Be(createdMessageByAliceCommand.Text);
        createdMessageByAlice.Value.ChatId.Should().Be(createdMessageByAliceCommand.ChatId);
        createdMessageByAlice.Value.OwnerDisplayName.Should().Be(alice.Value.DisplayName);
        createdMessageByAlice.Value.OwnerAvatarLink.Should().Be(alice.Value.AvatarLink);
        createdMessageByAlice.Value.ReplyToMessageId.Should().Be(createdMessageBy21Th.Value.Id);
        createdMessageByAlice.Value.ReplyToMessageText.Should().Be(createdMessageBy21Th.Value.Text);
        createdMessageByAlice.Value.ReplyToMessageAuthorDisplayName.Should().Be(user21Th.Value.DisplayName);
        
        createdMessageByBob.IsSuccess.Should().BeFalse();
    }
}