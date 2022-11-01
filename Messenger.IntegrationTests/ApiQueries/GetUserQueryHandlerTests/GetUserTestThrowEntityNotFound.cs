using FluentAssertions;
using Messenger.BusinessLogic.ApiQueries.Users;
using Messenger.BusinessLogic.Responses;
using Messenger.IntegrationTests.Abstraction;
using Xunit;

namespace Messenger.IntegrationTests.ApiQueries.GetUserQueryHandlerTests;

public class GetUserTestThrowEntityNotFound : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var getUserResult = await MessengerModule.RequestAsync(new GetUserQuery(
            UserId: Guid.NewGuid()), CancellationToken.None);

        getUserResult.Error.Should().BeOfType<DbEntityNotFoundError>();
    }
}