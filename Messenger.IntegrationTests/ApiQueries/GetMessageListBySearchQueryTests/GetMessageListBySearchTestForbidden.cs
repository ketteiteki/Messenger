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

        var createConversationCommand = new CreateChatCommand(
            user21Th.Value.Id,
            Name: "qwerty",
            Title: "qwerty",
            ChatType.Conversation,
            AvatarFile: null);
        
        var createConversationResult = await MessengerModule.RequestAsync(createConversationCommand, CancellationToken.None);

        var aliceJoinConversationCommand = new JoinToChatCommand(alice.Value.Id, createConversationResult.Value.Id);
        
        await MessengerModule.RequestAsync(aliceJoinConversationCommand, CancellationToken.None);

        var banAliceInConversationBy21ThCommand = new BanUserInConversationCommand(
            user21Th.Value.Id,
            createConversationResult.Value.Id,
            alice.Value.Id,
            BanMinutes: 15); 
        
        await MessengerModule.RequestAsync(banAliceInConversationBy21ThCommand, CancellationToken.None);

        var createMessageBy21ThCommand = new CreateMessageCommand(
            user21Th.Value.Id,
            Text: "text",
            ReplyToId: null,
            createConversationResult.Value.Id,
            Files: null);
        
        await MessengerModule.RequestAsync(createMessageBy21ThCommand, CancellationToken.None);

        var getMessageListBySearchByAliceQuery = new GetMessageListBySearchQuery(
            alice.Value.Id,
            createConversationResult.Value.Id,
            Limit: 10,
            FromMessageDateTime: null,
            createMessageBy21ThCommand.Text);
        
        var getMessageListByAliceResult = 
            await MessengerModule.RequestAsync(getMessageListBySearchByAliceQuery, CancellationToken.None);

        getMessageListByAliceResult.Error.Should().BeOfType<ForbiddenError>();
    }
}