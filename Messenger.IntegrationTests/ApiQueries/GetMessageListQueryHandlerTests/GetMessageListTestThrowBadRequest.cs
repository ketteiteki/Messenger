using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiQueries.Messages;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiQueries.GetMessageListQueryHandlerTests;

public class GetMessageListTestThrowBadRequest : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
        
        var conversation = await MessengerModule.RequestAsync(new CreateChatCommand(
            RequesterId: user21Th.Value.Id,
            Name: "qwerty",
            Title: "qwerty",
            Type: ChatType.Conversation,
            AvatarFile: null), CancellationToken.None);

        var getMessageListResult = await MessengerModule.RequestAsync(new GetMessageListQuery(
            RequesterId: user21Th.Value.Id,
            ChatId: conversation.Value.Id,
            Limit: 61,
            FromMessageDateTime: null), CancellationToken.None);

        getMessageListResult.Error.Should().BeOfType<BadRequestError>();
    }
}