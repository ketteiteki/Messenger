using FluentAssertions;
using Messenger.BusinessLogic.ApiQueries.Users;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiQueries.GetUserQueryHandlerTests;

public class GetUserTestSuccess : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);

        var getUserQuery = new GetUserQuery(user21Th.Value.Id);
        
        var getUserResult = await RequestAsync(getUserQuery, CancellationToken.None);

        getUserResult.Value.Id.Should().Be(user21Th.Value.Id);
        getUserResult.Value.Nickname.Should().Be(user21Th.Value.Nickname);
        getUserResult.Value.DisplayName.Should().Be(user21Th.Value.DisplayName);
        getUserResult.Value.Bio.Should().Be(user21Th.Value.Bio);
    }
}