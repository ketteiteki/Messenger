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
		var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);

		await MessengerModule.RequestAsync(new LoginCommand(
			NickName: user21Th.Value.Nickname,
			Password: "1234567890"), CancellationToken.None);
	}
}