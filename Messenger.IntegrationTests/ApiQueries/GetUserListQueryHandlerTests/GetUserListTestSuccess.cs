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
        var alice = await MessengerModule.RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);
        var bob = await MessengerModule.RequestAsync(CommandHelper.RegistrationBobCommand(), CancellationToken.None);
        var alex = await MessengerModule.RequestAsync(CommandHelper.RegistrationAlexCommand(), CancellationToken.None);

        var usersLimit2Page1 = await MessengerModule.RequestAsync(new GetUserListQuery(
            Limit: 2,
            Page: 1,
            SearchText: null), CancellationToken.None);
        
        var usersLimit2Page2 = await MessengerModule.RequestAsync(new GetUserListQuery(
            Limit: 2,
            Page: 2,
            SearchText: null), CancellationToken.None);
        
        var usersLimit2Page3 = await MessengerModule.RequestAsync(new GetUserListQuery(
            Limit: 2,
            Page: 3,
            SearchText: null), CancellationToken.None);

        var userWhoContains123InNickname = await MessengerModule.RequestAsync(new GetUserListQuery(
            Limit: 20,
            Page: 1,
            SearchText: "123"), CancellationToken.None);
        
        for (var i = 0; i < usersLimit2Page1.Value.Count; i++)
        {
            usersLimit2Page1.Value[i].Id.Should().NotBe(usersLimit2Page2.Value[i].Id);
        }

        usersLimit2Page3.Value.Count.Should().Be(0);

        userWhoContains123InNickname.Value.Count.Should().Be(3);
    }
}