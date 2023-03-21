using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Auth;
using Messenger.BusinessLogic.Responses;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.AuthorizationCommandHandlerTests;

public class AuthorizationTestThrowAuthentication : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);

        var authorizationCommand = new AuthorizationCommand(AuthorizationToken: "wrong token");
        
        var authorizationResult = await MessengerModule.RequestAsync(authorizationCommand, CancellationToken.None);

        authorizationResult.Error.Should().BeOfType<AuthenticationError>();
    }
}