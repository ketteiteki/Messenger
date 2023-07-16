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
        var user21Th = await RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);

        var getChatQuery = new GetChatQuery(user21Th.Value.Id, Guid.NewGuid());
        
        var getConversationResult = await RequestAsync(getChatQuery, CancellationToken.None);

        getConversationResult.Error.Should().BeOfType<DbEntityNotFoundError>();
    }
}