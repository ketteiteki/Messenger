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
        var firstLoginCommand = new LoginCommand(
            CommandHelper.Registration21ThCommand().Nickname,
            CommandHelper.Registration21ThCommand().Password);
        
        var firstLoginResult = await MessengerModule.RequestAsync(firstLoginCommand, CancellationToken.None);
        
        await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);

        var secondLoginCommand = new LoginCommand(
            CommandHelper.Registration21ThCommand().Nickname,
            Password: "wrong password");
        
        var secondLoginResult = await MessengerModule.RequestAsync(secondLoginCommand, CancellationToken.None);

        firstLoginResult.Error.Should().BeOfType<AuthenticationError>();
        secondLoginResult.Error.Should().BeOfType<AuthenticationError>();
    }
}