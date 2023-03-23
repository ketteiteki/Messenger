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

        var loginCommand = new LoginCommand(
            CommandHelper.Registration21ThCommand().Nickname,
            CommandHelper.Registration21ThCommand().Password,
            CommandHelper.Registration21ThCommand().Ip,
            CommandHelper.Registration21ThCommand().UserAgent);

        var loginResult = await MessengerModule.RequestAsync(loginCommand, CancellationToken.None);

        var getSessionByRegistrationQuery = new GetSessionQuery(user21Th.Value.Id, user21Th.Value.AccessToken);
        
        var getSessionByRegistrationResult = 
            await MessengerModule.RequestAsync(getSessionByRegistrationQuery, CancellationToken.None);

        var getSessionByLoginQuery = new GetSessionQuery(user21Th.Value.Id, loginResult.Value.AccessToken);
        
        var getSessionByLoginResult = await MessengerModule.RequestAsync(getSessionByLoginQuery, CancellationToken.None);

        getSessionByRegistrationResult.Value.Id.Should().NotBe(getSessionByLoginResult.Value.Id);
    }
}