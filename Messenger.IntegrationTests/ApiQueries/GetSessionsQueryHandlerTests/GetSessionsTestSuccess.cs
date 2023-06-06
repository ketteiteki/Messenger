using FluentAssertions;
using Messenger.BusinessLogic.ApiQueries.Auth;
using Messenger.Domain.Entities;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiQueries.GetSessionsQueryHandlerTests;

public class GetSessionsTestSuccess : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
        
        for (var i = 0; i < 4; i++)
        {
            var newUserSession = 
                new UserSessionEntity(Guid.NewGuid(), user21Th.Value.Id, DateTimeOffset.UtcNow, Array.Empty<byte>());

            DatabaseContextFixture.Add(newUserSession);
        }

        await DatabaseContextFixture.SaveChangesAsync();

        var getSessionsQuery = new GetSessionsQuery(user21Th.Value.Id);

        var getSessionsResult = await MessengerModule.RequestAsync(getSessionsQuery, CancellationToken.None);

        getSessionsResult.Value.Count.Should().Be(4);
    }
}