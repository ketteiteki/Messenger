using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.BusinessLogic.ApiCommands.Messages;
using Messenger.BusinessLogic.ApiQueries.Messages;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiQueries.GetMessageListBySearchQueryTests;

public class GetMessageListBySearchTestSuccess : IntegrationTestBase, IIntegrationTest
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

        var createMessageForSearchCheckCommand = new CreateMessageCommand(
            RequesterId: user21Th.Value.Id,
            Text: "qqwrrewr",
            ReplyToId: null,
            ChatId: conversation.Value.Id,
            Files: null);
        
        var createdMessageBy21Th = await MessengerModule.RequestAsync(createMessageForSearchCheckCommand, CancellationToken.None);
        
        await MessengerModule.RequestAsync(new CreateMessageCommand(
            RequesterId: alice.Value.Id,
            Text: "qre43qwrrfdewr",
            ReplyToId: null,
            ChatId: conversation.Value.Id,
            Files: null), CancellationToken.None);

        var messagesForAlice = await MessengerModule.RequestAsync(new GetMessageListBySearchQuery(
            RequesterId: alice.Value.Id,
            ChatId: conversation.Value.Id,
            Limit: 10,
            FromMessageDateTime: null,
            SearchText: createMessageForSearchCheckCommand.Text
        ), CancellationToken.None);
        
        var messagesForBob = await MessengerModule.RequestAsync(new GetMessageListBySearchQuery(
            RequesterId: bob.Value.Id,
            ChatId: conversation.Value.Id,
            Limit: 10,
            FromMessageDateTime: null,
            SearchText: createMessageForSearchCheckCommand.Text
        ), CancellationToken.None);

        messagesForAlice.Value.Count.Should().Be(1);
        messagesForAlice.Value[0].Id.Should().Be(createdMessageBy21Th.Value.Id);
        messagesForAlice.Value[0].Text.Should().Be(createdMessageBy21Th.Value.Text);
        messagesForAlice.Value[0].OwnerId.Should().Be(createdMessageBy21Th.Value.OwnerId);

        messagesForBob.Value.Count.Should().Be(1);
        messagesForBob.Value[0].Id.Should().Be(createdMessageBy21Th.Value.Id);
        messagesForBob.Value[0].Text.Should().Be(createdMessageBy21Th.Value.Text);
        messagesForBob.Value[0].OwnerId.Should().Be(createdMessageBy21Th.Value.OwnerId);
    }
}