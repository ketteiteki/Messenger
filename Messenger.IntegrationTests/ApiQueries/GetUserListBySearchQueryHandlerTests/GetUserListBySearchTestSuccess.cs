using FluentAssertions;
using Messenger.BusinessLogic.ApiQueries.Users;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiQueries.GetUserListBySearchQueryHandlerTests;

public class GetUserListBySearchTestSuccess : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
        await MessengerModule.RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);
        var bob = await MessengerModule.RequestAsync(CommandHelper.RegistrationBobCommand(), CancellationToken.None);

        var getUserListBySearchStringEmptyQuery = new GetUserListBySearchQuery(
            user21Th.Value.Id,
            string.Empty,
            Limit: 10,
            Page: 1);
        
        var getUserListBySearchStringEmptyResult = 
            await MessengerModule.RequestAsync(getUserListBySearchStringEmptyQuery, CancellationToken.None);

        var getUserListBySearchBobNicknameQuery = new GetUserListBySearchQuery(
            user21Th.Value.Id,
            bob.Value.Nickname,
            Limit: 10,
            Page: 1);
        
        var getUserListBySearchBobNicknameResult =
            await MessengerModule.RequestAsync(getUserListBySearchBobNicknameQuery, CancellationToken.None);
        
        getUserListBySearchStringEmptyResult.Value.Count.Should().Be(2);
        getUserListBySearchStringEmptyResult.Value.FirstOrDefault(u => u.Id == user21Th.Value.Id).Should().BeNull();

        getUserListBySearchBobNicknameResult.Value.Count.Should().Be(1);
        getUserListBySearchBobNicknameResult.Value.First(u => u.Id == bob.Value.Id).Should().NotBeNull();
    }
}