using Messenger.BusinessLogic.Auth.Commands;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.Auth.CommandsTests;

public class LoginCommandHandlerTestSuccess : IntegrationTestBase, IIntegrationTest
{
	[Fact]
	public async Task Test()
	{
		var user = await MessengerModule.RequestAsync(CommandHelper.Registration21thCommand(), CancellationToken.None);

		await MessengerModule.RequestAsync(new LoginCommand
		{
			NickName = user.Nickname,
			Password = "1234567890"
		}, CancellationToken.None);
	}
}