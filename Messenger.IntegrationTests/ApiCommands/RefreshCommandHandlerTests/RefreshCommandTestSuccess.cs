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

        var firstAliceLoginCommand = new LoginCommand(
            CommandHelper.RegistrationAliceCommand().Nickname,
            CommandHelper.RegistrationAliceCommand().Password,
            CommandHelper.RegistrationAliceCommand().Ip,
            CommandHelper.RegistrationAliceCommand().UserAgent);
        
        await MessengerModule.RequestAsync(firstAliceLoginCommand, CancellationToken.None);

        for (var i = 0; i < 9; i++)
        {
            var bobLoginCommand = new LoginCommand(
                Nickname: CommandHelper.RegistrationBobCommand().Nickname,
                Password: CommandHelper.RegistrationBobCommand().Password,
                Ip: CommandHelper.RegistrationBobCommand().Ip,
                UserAgent: CommandHelper.RegistrationBobCommand().UserAgent);
            
            await MessengerModule.RequestAsync(bobLoginCommand, CancellationToken.None);
        }

        var refreshBy21ThCommand = new RefreshCommand(
            UserAgent: "Google",
            Ip: "434.321.54.211",
            user21Th.Value.RefreshToken);
        
        var refreshBy21ThResult = await MessengerModule.RequestAsync(refreshBy21ThCommand, CancellationToken.None);

        var getSessionListBy21ThCommand = new GetSessionListQuery(user21Th.Value.Id);
        
        var getSessionListBy21ThResult = 
            await MessengerModule.RequestAsync(getSessionListBy21ThCommand, CancellationToken.None);

        var getSessionListByAliceCommand = new GetSessionListQuery(alice.Value.Id);
        
        var getSessionListByAliceResult = 
            await MessengerModule.RequestAsync(getSessionListByAliceCommand, CancellationToken.None);

        var getSessionListByBobCommand = new GetSessionListQuery(bob.Value.Id);
        
        var getSessionListByBobResult =
            await MessengerModule.RequestAsync(getSessionListByBobCommand, CancellationToken.None);
        
        refreshBy21ThResult.Value.RefreshToken.Should().NotBe(user21Th.Value.RefreshToken);
        refreshBy21ThResult.Value.AccessToken.Should().NotBe(user21Th.Value.AccessToken);

        getSessionListBy21ThResult.Value.Count.Should().Be(1);
        getSessionListByAliceResult.Value.Count.Should().Be(2);
        getSessionListByBobResult.Value.Count.Should().Be(7);
    }
}