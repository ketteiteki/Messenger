using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Auth;
using Messenger.BusinessLogic.ApiQueries.Auth;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.RefreshCommandHandlerTests;

public class RefreshCommandTestSuccess : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
        var alice = await MessengerModule.RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);
        var bob = await MessengerModule.RequestAsync(CommandHelper.RegistrationBobCommand(), CancellationToken.None);

        await MessengerModule.RequestAsync(new LoginCommand(
            Nickname: CommandHelper.RegistrationAliceCommand().Nickname,
            Password: CommandHelper.RegistrationAliceCommand().Password,
            Ip: CommandHelper.RegistrationAliceCommand().Ip,
            UserAgent: CommandHelper.RegistrationAliceCommand().UserAgent), CancellationToken.None);

        for (var i = 0; i < 9; i++)
        {
            await MessengerModule.RequestAsync(new LoginCommand(
                Nickname: CommandHelper.RegistrationBobCommand().Nickname,
                Password: CommandHelper.RegistrationBobCommand().Password,
                Ip: CommandHelper.RegistrationBobCommand().Ip,
                UserAgent: CommandHelper.RegistrationBobCommand().UserAgent), CancellationToken.None);
        }
        
        var refreshBy21ThResult = await MessengerModule.RequestAsync(new RefreshCommand(
            RefreshToken: user21Th.Value.RefreshToken,
            UserAgent: "Google",
            Ip: "434.321.54.211"), CancellationToken.None);

        var getSessionListBy21ThResult = await MessengerModule.RequestAsync(new GetSessionListQuery(
            RequesterId: user21Th.Value.Id), CancellationToken.None);
        
        var getSessionListByAliceResult = await MessengerModule.RequestAsync(new GetSessionListQuery(
            RequesterId: alice.Value.Id), CancellationToken.None);
        
        var getSessionListByBobResult = await MessengerModule.RequestAsync(new GetSessionListQuery(
            RequesterId: bob.Value.Id), CancellationToken.None);
        
        refreshBy21ThResult.Value.RefreshToken.Should().NotBe(user21Th.Value.RefreshToken);
        refreshBy21ThResult.Value.AccessToken.Should().NotBe(user21Th.Value.AccessToken);

        getSessionListBy21ThResult.Value.Count.Should().Be(1);
        getSessionListByAliceResult.Value.Count.Should().Be(2);
        getSessionListByBobResult.Value.Count.Should().Be(7);
    }
}