using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.BusinessLogic.ApiCommands.Messages;
using Messenger.BusinessLogic.ApiQueries.Messages;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiQueries.GetMessageListBySearchQueryTests;

public class GetMessageListBySearchTestForbidden : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
        var alice = await MessengerModule.RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);

        var conversation = await MessengerModule.RequestAsync(new CreateChatCommand(
            RequesterId: user21Th.Value.Id,
            Name: "qwerty",
            Title: "qwerty",
            Type: ChatType.Conversation,
            AvatarFile: null), CancellationToken.None);

        await MessengerModule.RequestAsync(new JoinToChatCommand(
            RequesterId: alice.Value.Id,
            ChatId: conversation.Value.Id), CancellationToken.None);
        
        await MessengerModule.RequestAsync(new BanUserInConversationCommand(
            RequesterId: user21Th.Value.Id,
            ChatId: conversation.Value.Id,
            UserId: alice.Value.Id,
            BanDateOfExpire: DateTime.UtcNow.AddDays(2)), CancellationToken.None);
        
        var createMessageForSearchCheckCommand = new CreateMessageCommand(
            RequesterId: user21Th.Value.Id,
            Text: "text",
            ReplyToId: null,
            ChatId: conversation.Value.Id,
            Files: null);
        
        await MessengerModule.RequestAsync(createMessageForSearchCheckCommand, CancellationToken.None);
        
        var getMessageListByAliceResult = await MessengerModule.RequestAsync(new GetMessageListBySearchQuery(
            RequesterId: alice.Value.Id,
            ChatId: conversation.Value.Id,
            Limit: 10,
            FromMessageDateTime: null,
            SearchText: createMessageForSearchCheckCommand.Text
        ), CancellationToken.None);

        getMessageListByAliceResult.Error.Should().BeOfType<ForbiddenError>();
    }
}