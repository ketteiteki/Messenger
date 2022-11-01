using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Auth;
using Messenger.BusinessLogic.ApiQueries.Auth;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiQueries.GetSessionQueryHandlerTests;

public class GetSessionTestSuccess : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);

        var loginResult = await MessengerModule.RequestAsync(new LoginCommand(
            Nickname: CommandHelper.Registration21ThCommand().Nickname,
            Password: CommandHelper.Registration21ThCommand().Password,
            Ip: CommandHelper.Registration21ThCommand().Ip,
            UserAgent: CommandHelper.Registration21ThCommand().UserAgent), CancellationToken.None);

        var getSessionByRegistrationResult = await MessengerModule.RequestAsync(new GetSessionQuery(
            RequesterId: user21Th.Value.Id,
            AccessToken: user21Th.Value.AccessToken), CancellationToken.None);
        
        var getSessionByLoginResult = await MessengerModule.RequestAsync(new GetSessionQuery(
            RequesterId: user21Th.Value.Id,
            AccessToken: loginResult.Value.AccessToken), CancellationToken.None);

        getSessionByRegistrationResult.Value.Id.Should().NotBe(getSessionByLoginResult.Value.Id);
    }
}