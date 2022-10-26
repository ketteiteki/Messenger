using FluentAssertions;
using Messenger.BusinessLogic.ApiQueries.Auth;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiQueries.AuthorizationCommandHandlerTests;

public class AuthorizationTestSuccess : IntegrationTestBase, IIntegrationTest
{
	[Fact]
	public async Task Test()
	{
		var user21Th = await MessengerModule.RequestAsync
			(CommandHelper.Registration21ThCommand(), CancellationToken.None);

		var authorizationResult = await MessengerModule.RequestAsync
			(new AuthorizationCommand(
				AuthorizationToken: user21Th.Value.AccessToken), CancellationToken.None);

		authorizationResult.IsSuccess.Should().BeTrue();
	}
}