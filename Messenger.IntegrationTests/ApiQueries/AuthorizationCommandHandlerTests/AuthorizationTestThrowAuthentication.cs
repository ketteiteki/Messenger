using FluentAssertions;
using Messenger.BusinessLogic.ApiQueries.Auth;
using Messenger.BusinessLogic.Responses;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiQueries.AuthorizationCommandHandlerTests;

public class AuthorizationTestThrowAuthentication : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);

        var authorizationResult = await MessengerModule.RequestAsync(new AuthorizationCommand(
            AuthorizationToken: "wrong token"), CancellationToken.None);

        authorizationResult.Error.Should().BeOfType<AuthenticationError>();
    }
}