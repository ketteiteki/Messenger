using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiQueries.Messages;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Enums;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiQueries.GetMessageListBySearchQueryTests;

public class GetMessageListBySearchTestThrowBadRequest : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);

        var createConversationCommand = new CreateChatCommand(
            user21Th.Value.Id,
            Name: "qwerty",
            Title: "qwerty",
            ChatType.Conversation,
            AvatarFile: null);
        
        var createConversationResult = await MessengerModule.RequestAsync(createConversationCommand, CancellationToken.None);

        var getMessageListBySearchQuery = new GetMessageListBySearchQuery(
            user21Th.Value.Id,
            createConversationResult.Value.Id,
            Limit: 61,
            FromMessageDateTime: null,
            string.Empty);
        
        var getMessageListResult = 
            await MessengerModule.RequestAsync(getMessageListBySearchQuery, CancellationToken.None);

        getMessageListResult.Error.Should().BeOfType<BadRequestError>();
    }
}