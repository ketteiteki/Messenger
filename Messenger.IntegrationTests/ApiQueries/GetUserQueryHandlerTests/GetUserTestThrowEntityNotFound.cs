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
        var getUserQuery = new GetUserQuery(Guid.NewGuid());
        
        var getUserResult = await RequestAsync(getUserQuery, CancellationToken.None);

        getUserResult.Error.Should().BeOfType<DbEntityNotFoundError>();
    }
}