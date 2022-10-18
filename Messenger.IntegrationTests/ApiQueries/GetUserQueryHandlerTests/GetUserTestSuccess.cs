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
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);

        var user = await MessengerModule.RequestAsync(new GetUserQuery(
            UserId: user21Th.Value.Id), CancellationToken.None);

        user.Value.Id.Should().Be(user21Th.Value.Id);
        user.Value.Nickname.Should().Be(user21Th.Value.Nickname);
        user.Value.DisplayName.Should().Be(user21Th.Value.DisplayName);
        user.Value.Bio.Should().Be(user21Th.Value.Bio);
    }
}