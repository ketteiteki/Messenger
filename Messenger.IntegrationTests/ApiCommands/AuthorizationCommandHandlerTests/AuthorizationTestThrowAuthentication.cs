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
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);

        var authorizationCommand = new AuthorizationCommand(user21Th.Value.Id, AuthorizationToken: "wrong token");
        
        var authorizationResult = await MessengerModule.RequestAsync(authorizationCommand, CancellationToken.None);

        authorizationResult.Error.Should().BeOfType<AuthenticationError>();
    }
}