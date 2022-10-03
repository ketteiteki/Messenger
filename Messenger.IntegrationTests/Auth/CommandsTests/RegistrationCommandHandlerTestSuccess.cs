using Messenger.BusinessLogic.Auth.Commands;
using Xunit;

namespace Messenger.IntegrationTests.Auth.CommandsTests;

public class RegistrationCommandHandlerTestSuccess : IntegrationTestBase 
{
	[Fact]
	public async Task Test()
	{
		var command = new RegistrationCommand
		{
			DisplayName = "D1F492gfdgfdlkgld",
			Nickname = "D1F492gfdgfdlkgld",
			Password = "D1F492gfdgfdlkgld",
		};

		await MessengerModule.RequestAsync(command, CancellationToken.None);
	}
}