using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Messages;
using Messenger.BusinessLogic.ApiQueries.Messages;
using Messenger.Domain.Enum;
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
        
        var createMessageBy21ThResult = await MessengerModule.RequestAsync(new CreateMessageCommand(
            RequesterId: user21Th.Value.Id,
            Text: "text1",
            ReplyToId: null,
            ChatId: conversation.Value.Id,
            Files: null), CancellationToken.None);
        
        var createMessageByAliceResult = await MessengerModule.RequestAsync(new CreateMessageCommand(
            RequesterId: alice.Value.Id,
            Text: "text2",
            ReplyToId: null,
            ChatId: conversation.Value.Id,
            Files: null), CancellationToken.None);

        var getMessageListByAliceResult = await MessengerModule.RequestAsync(new GetMessageListQuery(
            RequesterId: alice.Value.Id,
            ChatId: conversation.Value.Id,
            Limit: 10,
            FromMessageDateTime: null 
            ), CancellationToken.None);

        var getMessageListByBobResult = await MessengerModule.RequestAsync(new GetMessageListQuery(
            RequesterId: bob.Value.Id,
            ChatId: conversation.Value.Id,
            Limit: 10,
            FromMessageDateTime: null 
        ), CancellationToken.None);

        var getMessageListFromMessageDateTimeByAliceResult = await MessengerModule.RequestAsync(new GetMessageListQuery(
            RequesterId: alice.Value.Id,
            ChatId: conversation.Value.Id,
            Limit: 10,
            FromMessageDateTime: createMessageByAliceResult.Value.DateOfCreate 
        ), CancellationToken.None);
        
        for (var i = 0; i < getMessageListByAliceResult.Value.Count; i++)
        {
            getMessageListByAliceResult.Value[i].Id.Should().Be(getMessageListByBobResult.Value[i].Id);
            getMessageListByAliceResult.Value[i].Text.Should().Be(getMessageListByBobResult.Value[i].Text);
            getMessageListByAliceResult.Value[i].ChatId.Should().Be(getMessageListByBobResult.Value[i].ChatId);
            getMessageListByAliceResult.Value[i].OwnerId.Should().Be(getMessageListByBobResult.Value[i].OwnerId);
            getMessageListByAliceResult.Value[i].ReplyToMessageId.Should().Be(getMessageListByBobResult.Value[i].ReplyToMessageId);
        }

        getMessageListFromMessageDateTimeByAliceResult.Value.Count.Should().Be(1);
        getMessageListFromMessageDateTimeByAliceResult.Value[0].Id.Should().Be(createMessageBy21ThResult.Value.Id);
        getMessageListFromMessageDateTimeByAliceResult.Value[0].Text.Should().Be(createMessageBy21ThResult.Value.Text);
        getMessageListFromMessageDateTimeByAliceResult.Value[0].OwnerId.Should().Be(createMessageBy21ThResult.Value.OwnerId);
    }
}