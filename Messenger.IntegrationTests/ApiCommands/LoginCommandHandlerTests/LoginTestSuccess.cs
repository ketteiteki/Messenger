using Messenger.BusinessLogic.ApiCommands.Auth;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.LoginCommandHandlerTests;

public class LoginTestSuccess : IntegrationTestBase, IIntegrationTest
{
	[Fact]
	public async Task Test()
	{
		var user21tth = await MessengerModule.RequestAsync(CommandHelper.Registration21thCommand(), CancellationToken.None);

		await MessengerModule.RequestAsync(new LoginCommand(
			NickName: user21tth.Value.Nickname,
			Password: "1234567890"), CancellationToken.None);
	}
}