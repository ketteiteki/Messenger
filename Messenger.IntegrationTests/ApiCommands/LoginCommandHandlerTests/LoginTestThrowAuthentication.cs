using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Auth;
using Messenger.BusinessLogic.Responses;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.LoginCommandHandlerTests;

public class LoginTestThrowAuthentication : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        const string loginIp = "323.432.21.542";
        
        var firstLoginResult = await MessengerModule.RequestAsync(new LoginCommand(
            Nickname: CommandHelper.Registration21ThCommand().Nickname,
            Password: CommandHelper.Registration21ThCommand().Password,
            UserAgent: "Mozilla",
            Ip: loginIp), CancellationToken.None);
        
        await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
   
        var secondLoginResult = await MessengerModule.RequestAsync(new LoginCommand(
            Nickname: CommandHelper.Registration21ThCommand().Nickname,
            Password: "wrong password",
            UserAgent: "Mozilla",
            Ip: loginIp), CancellationToken.None);

        firstLoginResult.Error.Should().BeOfType<AuthenticationError>();
        secondLoginResult.Error.Should().BeOfType<AuthenticationError>();
    }
}