using FluentAssertions;
using Messenger.BusinessLogic.ApiQueries.Chats;
using Messenger.BusinessLogic.Responses;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiQueries.GetChatQueryHandlerTests;

public class GetConversationTestThrowEntityNotFound : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);

        var getConversationResult = await MessengerModule.RequestAsync(new GetChatQuery(
            RequesterId: user21Th.Value.Id,
            ChatId: Guid.NewGuid()), CancellationToken.None);

        getConversationResult.Error.Should().BeOfType<DbEntityNotFoundError>();
    }
}