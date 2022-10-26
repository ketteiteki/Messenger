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
        var firstLoginResult = await MessengerModule.RequestAsync(new LoginCommand(
            NickName: CommandHelper.Registration21ThCommand().Nickname,
            Password: CommandHelper.Registration21ThCommand().Password), CancellationToken.None);
        
        await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
   
        var secondLoginResult = await MessengerModule.RequestAsync(new LoginCommand(
            NickName: CommandHelper.Registration21ThCommand().Nickname,
            Password: "wrong password"), CancellationToken.None);

        firstLoginResult.Error.Should().BeOfType<AuthenticationError>();
        secondLoginResult.Error.Should().BeOfType<AuthenticationError>();
    }
}