using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Auth;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.AuthorizationCommandHandlerTests;

public class AuthorizationTestSuccess : IntegrationTestBase, IIntegrationTest
{
	[Fact]
	public async Task Test()
	{
		var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);

		var authorizationCommand = new AuthorizationCommand(user21Th.Value.AccessToken);
		
		var authorizationResult = await MessengerModule.RequestAsync(authorizationCommand, CancellationToken.None);

		authorizationResult.IsSuccess.Should().BeTrue();
	}
}