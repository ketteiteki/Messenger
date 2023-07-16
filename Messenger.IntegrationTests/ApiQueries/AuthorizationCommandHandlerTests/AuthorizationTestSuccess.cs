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
		var user21Th = await RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
		
		var authorizationCommand = new AuthorizationCommand(user21Th.Value.Id);
		
		var authorizationResult = await RequestAsync(authorizationCommand, CancellationToken.None);
		
		authorizationResult.IsSuccess.Should().BeTrue();
	}
}