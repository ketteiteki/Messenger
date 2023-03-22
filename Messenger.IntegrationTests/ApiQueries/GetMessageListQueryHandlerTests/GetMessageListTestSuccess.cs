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
            user21Th.Value.Id,
            Name: "qwerty",
            Title: "qwerty",
            ChatType.Conversation,
            AvatarFile: null);

        var createConversationResult = 
            await MessengerModule.RequestAsync(createConversationCommand, CancellationToken.None);

        var aliceJoinConversationCommand = new JoinToChatCommand(alice.Value.Id, createConversationResult.Value.Id);
        
        await MessengerModule.RequestAsync(aliceJoinConversationCommand, CancellationToken.None);
        
        var createMessageBy21ThCommand = new CreateMessageCommand(
            user21Th.Value.Id,
            Text: "text1",
            ReplyToId: null,
            createConversationResult.Value.Id,
            Files: null);
        
        var createMessageBy21ThResult = 
            await MessengerModule.RequestAsync(createMessageBy21ThCommand, CancellationToken.None);

        var createMessageByAliceCommand = new CreateMessageCommand(
            alice.Value.Id,
            Text: "text2",
            ReplyToId: null,
            createConversationResult.Value.Id,
            Files: null);
        
        var createMessageByAliceResult = await MessengerModule.RequestAsync(createMessageByAliceCommand, CancellationToken.None);

        var firstGetMessageListByAliceQuery = new GetMessageListQuery(
            alice.Value.Id,
            createConversationResult.Value.Id,
            Limit: 10,
            FromMessageDateTime: null);
        
        var firstGetMessageListByAliceResult = 
            await MessengerModule.RequestAsync(firstGetMessageListByAliceQuery, CancellationToken.None);

        var getMessageListByBobQuery = new GetMessageListQuery(
            bob.Value.Id,
            createConversationResult.Value.Id,
            Limit: 10,
            FromMessageDateTime: null
        );
        
        var getMessageListByBobResult = 
            await MessengerModule.RequestAsync(getMessageListByBobQuery, CancellationToken.None);

        var getMessageListFromMessageDateTimeByAliceQuery = new GetMessageListQuery(
            alice.Value.Id,
            createConversationResult.Value.Id,
            Limit: 10,
            createMessageByAliceResult.Value.DateOfCreate);
        
        var getMessageListFromMessageDateTimeByAliceResult =
            await MessengerModule.RequestAsync(getMessageListFromMessageDateTimeByAliceQuery, CancellationToken.None);
        
        for (var i = 0; i < firstGetMessageListByAliceResult.Value.Count; i++)
        {
            firstGetMessageListByAliceResult.Value[i].Id.Should().Be(getMessageListByBobResult.Value[i].Id);
            firstGetMessageListByAliceResult.Value[i].Text.Should().Be(getMessageListByBobResult.Value[i].Text);
            firstGetMessageListByAliceResult.Value[i].ChatId.Should().Be(getMessageListByBobResult.Value[i].ChatId);
            firstGetMessageListByAliceResult.Value[i].OwnerId.Should().Be(getMessageListByBobResult.Value[i].OwnerId);
            firstGetMessageListByAliceResult.Value[i].ReplyToMessageId
                .Should().Be(getMessageListByBobResult.Value[i].ReplyToMessageId);
        }

        getMessageListFromMessageDateTimeByAliceResult.Value.Count.Should().Be(1);
        getMessageListFromMessageDateTimeByAliceResult.Value[0].Id.Should().Be(createMessageBy21ThResult.Value.Id);
        getMessageListFromMessageDateTimeByAliceResult.Value[0].Text.Should().Be(createMessageBy21ThResult.Value.Text);
        getMessageListFromMessageDateTimeByAliceResult.Value[0].OwnerId
            .Should().Be(createMessageBy21ThResult.Value.OwnerId);

        var delete21ThMessageBy21ThCommand = new DeleteMessageCommand(
            user21Th.Value.Id,
            createMessageBy21ThResult.Value.Id,
            IsDeleteForAll: false);
        
        await MessengerModule.RequestAsync(delete21ThMessageBy21ThCommand, CancellationToken.None);

        var firstGetMessageListBy21ThQuery = new GetMessageListQuery(
            user21Th.Value.Id,
            createConversationResult.Value.Id,
            Limit: 10,
            FromMessageDateTime: null);
        
        var firstGetMessageListBy21ThResult = 
            await MessengerModule.RequestAsync(firstGetMessageListBy21ThQuery, CancellationToken.None);

        var secondGetMessageListByAliceQuery = new GetMessageListQuery(
            alice.Value.Id,
            createConversationResult.Value.Id,
            Limit: 10,
            FromMessageDateTime: null);

        var secondGetMessageListByAliceResult = 
            await MessengerModule.RequestAsync(secondGetMessageListByAliceQuery, CancellationToken.None);
        
        firstGetMessageListBy21ThResult.Value
            .FirstOrDefault(m => m.Id == createMessageBy21ThResult.Value.Id).Should().BeNull();
        secondGetMessageListByAliceResult.Value
            .FirstOrDefault(m => m.Id == createMessageBy21ThResult.Value.Id).Should().NotBeNull();

        var deleteAliceMessageByAliceCommand = new DeleteMessageCommand(
            alice.Value.Id,
            createMessageByAliceResult.Value.Id,
            IsDeleteForAll: true); 
        
        await MessengerModule.RequestAsync(deleteAliceMessageByAliceCommand, CancellationToken.None);

        var secondGetMessageListBy21ThQuery = new GetMessageListQuery(
            user21Th.Value.Id,
            createConversationResult.Value.Id,
            Limit: 10,
            FromMessageDateTime: null);
        
        var secondGetMessageListBy21ThResult = 
            await MessengerModule.RequestAsync(secondGetMessageListBy21ThQuery, CancellationToken.None);

        var thirdGetMessageListByAliceQuery = new GetMessageListQuery(
            alice.Value.Id,
            createConversationResult.Value.Id,
            Limit: 10,
            FromMessageDateTime: null);
        
        var thirdGetMessageListByAliceResult =
            await MessengerModule.RequestAsync(thirdGetMessageListByAliceQuery, CancellationToken.None);
        
        secondGetMessageListBy21ThResult.Value
            .FirstOrDefault(m => m.Id == createMessageByAliceResult.Value.Id).Should().BeNull();
        thirdGetMessageListByAliceResult.Value
            .FirstOrDefault(m => m.Id == createMessageByAliceResult.Value.Id).Should().BeNull();
    }
}