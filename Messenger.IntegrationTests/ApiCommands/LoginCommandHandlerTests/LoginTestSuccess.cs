using FluentAssertions;
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
		await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);

		var loginCommand = new LoginCommand(
			CommandHelper.Registration21ThCommand().Nickname,
			CommandHelper.Registration21ThCommand().Password,
			Ip: "323.432.21.542",
			UserAgent: "Mozilla");
		
		var loginResult = await MessengerModule.RequestAsync(loginCommand, CancellationToken.None);

		loginResult.IsSuccess.Should().BeTrue();
	}
}