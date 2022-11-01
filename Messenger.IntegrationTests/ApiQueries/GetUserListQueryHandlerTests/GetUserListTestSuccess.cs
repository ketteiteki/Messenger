using FluentAssertions;
using Messenger.BusinessLogic.ApiQueries.Users;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiQueries.GetUserListQueryHandlerTests;

public class GetUserListTestSuccess : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
        await MessengerModule.RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);
        await MessengerModule.RequestAsync(CommandHelper.RegistrationBobCommand(), CancellationToken.None);
        await MessengerModule.RequestAsync(CommandHelper.RegistrationAlexCommand(), CancellationToken.None);

        var getUserListLimit2Page1Result = await MessengerModule.RequestAsync(new GetUserListBySearchQuery(
            RequesterId: user21Th.Value.Id,
            Limit: 2,
            Page: 1,
            SearchText: null), CancellationToken.None);
        
        var getUserListLimit2Page2Result = await MessengerModule.RequestAsync(new GetUserListBySearchQuery(
            RequesterId: user21Th.Value.Id,
            Limit: 2,
            Page: 2,
            SearchText: null), CancellationToken.None);
        
        var getUserListLimit2Page3Result = await MessengerModule.RequestAsync(new GetUserListBySearchQuery(
            RequesterId: user21Th.Value.Id,
            Limit: 2,
            Page: 3,
            SearchText: null), CancellationToken.None);

        var getUserListBySearchResult = await MessengerModule.RequestAsync(new GetUserListBySearchQuery(
            RequesterId: user21Th.Value.Id,
            Limit: 20,
            Page: 1,
            SearchText: "123"), CancellationToken.None);
        
        for (var i = 0; i < getUserListLimit2Page1Result.Value.Count; i++)
        {
            getUserListLimit2Page1Result.Value[i].Id.Should().NotBe(getUserListLimit2Page2Result.Value[i].Id);
        }

        getUserListLimit2Page3Result.Value.Count.Should().Be(0);

        getUserListBySearchResult.Value.Count.Should().Be(3);
    }
}