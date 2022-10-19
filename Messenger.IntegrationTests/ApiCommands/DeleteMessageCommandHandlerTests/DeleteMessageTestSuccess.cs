using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.BusinessLogic.ApiCommands.Messages;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.DeleteMessageCommandHandlerTests;

public class DeleteMessageTestSuccess : IntegrationTestBase, IIntegrationTest
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
        
        await MessengerModule.RequestAsync(new JoinToChatCommand(
            RequesterId: bob.Value.Id,
            ChatId: conversation.Value.Id), CancellationToken.None);

        var createdFirstMessageByAlice = await MessengerModule.RequestAsync(new CreateMessageCommand(
            RequesterId: alice.Value.Id,
            Text: "qwerty2",
            ReplyToId: null,
            ChatId: conversation.Value.Id,
            Files: null), CancellationToken.None);

        var createdSecondMessageByAlice = await MessengerModule.RequestAsync(new CreateMessageCommand(
            RequesterId: alice.Value.Id,
            Text: "qwerty422",
            ReplyToId: null,
            ChatId: conversation.Value.Id,
            Files: null), CancellationToken.None);
        
        var deletedMessageAliceByBob = await MessengerModule.RequestAsync(new DeleteMessageCommand(
            RequesterId: bob.Value.Id,
            MessageId: createdFirstMessageByAlice.Value.Id,
            IsDeleteForAll: true), CancellationToken.None);
        
        var deletedMessageAliceBy21Th = await MessengerModule.RequestAsync(new DeleteMessageCommand(
            RequesterId: user21Th.Value.Id,
            MessageId: createdFirstMessageByAlice.Value.Id,
            IsDeleteForAll: true), CancellationToken.None);
        
        var deletedMessageAliceByAlice = await MessengerModule.RequestAsync(new DeleteMessageCommand(
            RequesterId: user21Th.Value.Id,
            MessageId: createdSecondMessageByAlice.Value.Id,
            IsDeleteForAll: true), CancellationToken.None);

        deletedMessageAliceByBob.IsSuccess.Should().BeFalse();

        deletedMessageAliceBy21Th.IsSuccess.Should().BeTrue();
        
        deletedMessageAliceByAlice.IsSuccess.Should().BeTrue();
    }
}