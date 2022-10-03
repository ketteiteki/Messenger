using Messenger.BusinessLogic.Auth.Queries;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.Auth.QueriesTests;

public class AuthorizationCommandHandlerTestSuccess : IntegrationTestBase, IIntegrationTest
{
	[Fact]
	public async Task Test()
	{
		var registrationUser = await MessengerModule.RequestAsync
			(CommandHelper.Registration21thCommand(), CancellationToken.None);

		await MessengerModule.RequestAsync
			(new AuthorizationCommand {AuthorizationToken = registrationUser.AccessToken}, CancellationToken.None);
	}
}