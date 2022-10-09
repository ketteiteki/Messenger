using Messenger.BusinessLogic.ApiCommands.Auth;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.RegistrationCommandHandlerTests;

public class RegistrationTestSuccess : IntegrationTestBase 
{
	[Fact]
	public async Task Test()
	{
		var command = new RegistrationCommand(
			DisplayName: "D1F492gfdgfdlkgld",
			Nickname: "D1F492gfdgfdlkgld",
			Password: "D1F492gfdgfdlkgld");

		await MessengerModule.RequestAsync(command, CancellationToken.None);
	}
}