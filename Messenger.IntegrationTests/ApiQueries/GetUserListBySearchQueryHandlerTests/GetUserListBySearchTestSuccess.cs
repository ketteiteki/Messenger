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

        var getUserListBySearchStringEmptyResult = await MessengerModule.RequestAsync(new GetUserListBySearchQuery(
            RequesterId: user21Th.Value.Id, 
            Limit: 10,
            Page: 1,
            SearchText: string.Empty), CancellationToken.None);

        var getUserListBySearchBobNicknameResult = await MessengerModule.RequestAsync(new GetUserListBySearchQuery(
            RequesterId: user21Th.Value.Id, 
            Limit: 10,
            Page: 1,
            SearchText: bob.Value.NickName), CancellationToken.None);
        
        getUserListBySearchStringEmptyResult.Value.Count.Should().Be(2);
        getUserListBySearchStringEmptyResult.Value.FirstOrDefault(u => u.Id == user21Th.Value.Id).Should().BeNull();

        getUserListBySearchBobNicknameResult.Value.Count.Should().Be(1);
        getUserListBySearchBobNicknameResult.Value.First(u => u.Id == bob.Value.Id).Should().NotBeNull();
    }
}