using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.BusinessLogic.ApiCommands.Messages;
using Messenger.BusinessLogic.ApiQueries.Messages;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiQueries.GetMessageListQueryHandlerTests;

public class GetMessageListTestSuccess : IntegrationTestBase, IIntegrationTest
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
            Text: "qqwrrewr",
            ReplyToId: null,
            ChatId: conversation.Value.Id,
            Files: null), CancellationToken.None);
        
        var createdMessageByAlice = await MessengerModule.RequestAsync(new CreateMessageCommand(
            RequesterId: alice.Value.Id,
            Text: "qre43qwrrfdewr",
            ReplyToId: null,
            ChatId: conversation.Value.Id,
            Files: null), CancellationToken.None);

        var messagesForAlice = await MessengerModule.RequestAsync(new GetMessageListQuery(
            RequesterId: alice.Value.Id,
            ChatId: conversation.Value.Id,
            Limit: 10,
            FromMessageDateTime: null 
            ), CancellationToken.None);

        var messagesForBob = await MessengerModule.RequestAsync(new GetMessageListQuery(
            RequesterId: bob.Value.Id,
            ChatId: conversation.Value.Id,
            Limit: 10,
            FromMessageDateTime: null 
        ), CancellationToken.None);

        var messagesForAliceFromMessageByAliceDataTime = await MessengerModule.RequestAsync(new GetMessageListQuery(
            RequesterId: alice.Value.Id,
            ChatId: conversation.Value.Id,
            Limit: 10,
            FromMessageDateTime: createdMessageByAlice.Value.DateOfCreate 
        ), CancellationToken.None);
        
        for (var i = 0; i < messagesForAlice.Value.Count; i++)
        {
            messagesForAlice.Value[i].Id.Should().Be(messagesForBob.Value[i].Id);
            messagesForAlice.Value[i].Text.Should().Be(messagesForBob.Value[i].Text);
            messagesForAlice.Value[i].ChatId.Should().Be(messagesForBob.Value[i].ChatId);
            messagesForAlice.Value[i].OwnerId.Should().Be(messagesForBob.Value[i].OwnerId);
            messagesForAlice.Value[i].ReplyToMessageId.Should().Be(messagesForBob.Value[i].ReplyToMessageId);
        }

        messagesForAliceFromMessageByAliceDataTime.Value.Count.Should().Be(1);
        messagesForAliceFromMessageByAliceDataTime.Value[0].Id.Should().Be(createdMessageBy21Th.Value.Id);
        messagesForAliceFromMessageByAliceDataTime.Value[0].Text.Should().Be(createdMessageBy21Th.Value.Text);
        messagesForAliceFromMessageByAliceDataTime.Value[0].OwnerId.Should().Be(createdMessageBy21Th.Value.OwnerId);
    }
}