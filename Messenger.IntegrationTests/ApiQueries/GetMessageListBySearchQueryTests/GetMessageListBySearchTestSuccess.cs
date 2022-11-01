using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Messages;
using Messenger.BusinessLogic.ApiQueries.Messages;
using Messenger.Domain.Enum;
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

        var createMessageForSearchCheckBy21ThCommand = new CreateMessageCommand(
            RequesterId: user21Th.Value.Id,
            Text: "text1",
            ReplyToId: null,
            ChatId: conversation.Value.Id,
            Files: null);
        
        var createMessageForSearchCheckBy21ThResult = 
            await MessengerModule.RequestAsync(createMessageForSearchCheckBy21ThCommand, CancellationToken.None);
        
        var createMessageForSearchByAliceResult = await MessengerModule.RequestAsync(new CreateMessageCommand(
            RequesterId: alice.Value.Id,
            Text: "text2",
            ReplyToId: null,
            ChatId: conversation.Value.Id,
            Files: null), CancellationToken.None);

        var firstGetMessageListBySearchByAliceResult = await MessengerModule.RequestAsync(new GetMessageListBySearchQuery(
            RequesterId: alice.Value.Id,
            ChatId: conversation.Value.Id,
            Limit: 10,
            FromMessageDateTime: null,
            SearchText: createMessageForSearchCheckBy21ThCommand.Text
        ), CancellationToken.None);
        
        var getMessageListBySearchByBobResult = await MessengerModule.RequestAsync(new GetMessageListBySearchQuery(
            RequesterId: bob.Value.Id,
            ChatId: conversation.Value.Id,
            Limit: 10,
            FromMessageDateTime: null,
            SearchText: createMessageForSearchCheckBy21ThCommand.Text
        ), CancellationToken.None);

        firstGetMessageListBySearchByAliceResult.Value.Count.Should().Be(1);
        firstGetMessageListBySearchByAliceResult.Value[0].Id.Should().Be(createMessageForSearchCheckBy21ThResult.Value.Id);
        firstGetMessageListBySearchByAliceResult.Value[0].Text.Should().Be(createMessageForSearchCheckBy21ThResult.Value.Text);
        firstGetMessageListBySearchByAliceResult.Value[0].OwnerId.Should().Be(createMessageForSearchCheckBy21ThResult.Value.OwnerId);

        getMessageListBySearchByBobResult.Value.Count.Should().Be(1);
        getMessageListBySearchByBobResult.Value[0].Id.Should().Be(createMessageForSearchCheckBy21ThResult.Value.Id);
        getMessageListBySearchByBobResult.Value[0].Text.Should().Be(createMessageForSearchCheckBy21ThResult.Value.Text);
        getMessageListBySearchByBobResult.Value[0].OwnerId.Should().Be(createMessageForSearchCheckBy21ThResult.Value.OwnerId);
        
        await MessengerModule.RequestAsync(new DeleteMessageCommand(
            RequesterId: user21Th.Value.Id,
            MessageId: createMessageForSearchCheckBy21ThResult.Value.Id,
            IsDeleteForAll: false), CancellationToken.None);
        
        var firstGetMessageListBy21ThResult = await MessengerModule.RequestAsync(new GetMessageListQuery(
            RequesterId: user21Th.Value.Id,
            ChatId: conversation.Value.Id,
            Limit: 10,
            FromMessageDateTime: null 
        ), CancellationToken.None);
        
        var secondGetMessageListByAliceResult = await MessengerModule.RequestAsync(new GetMessageListQuery(
            RequesterId: alice.Value.Id,
            ChatId: conversation.Value.Id,
            Limit: 10,
            FromMessageDateTime: null 
        ), CancellationToken.None);
        
        firstGetMessageListBy21ThResult.Value
            .FirstOrDefault(m => m.Id == createMessageForSearchCheckBy21ThResult.Value.Id).Should().BeNull();
        secondGetMessageListByAliceResult.Value
            .FirstOrDefault(m => m.Id == createMessageForSearchCheckBy21ThResult.Value.Id).Should().NotBeNull();
        
        await MessengerModule.RequestAsync(new DeleteMessageCommand(
            RequesterId: alice.Value.Id,
            MessageId: createMessageForSearchByAliceResult.Value.Id,
            IsDeleteForAll: true), CancellationToken.None);
        
        var secondGetMessageListBy21ThResult = await MessengerModule.RequestAsync(new GetMessageListQuery(
            RequesterId: user21Th.Value.Id,
            ChatId: conversation.Value.Id,
            Limit: 10,
            FromMessageDateTime: null 
        ), CancellationToken.None);
        
        var thirdGetMessageListByAliceResult = await MessengerModule.RequestAsync(new GetMessageListQuery(
            RequesterId: alice.Value.Id,
            ChatId: conversation.Value.Id,
            Limit: 10,
            FromMessageDateTime: null 
        ), CancellationToken.None);
        
        secondGetMessageListBy21ThResult.Value
            .FirstOrDefault(m => m.Id == createMessageForSearchByAliceResult.Value.Id).Should().BeNull();
        thirdGetMessageListByAliceResult.Value
            .FirstOrDefault(m => m.Id == createMessageForSearchByAliceResult.Value.Id).Should().BeNull();
    }
}