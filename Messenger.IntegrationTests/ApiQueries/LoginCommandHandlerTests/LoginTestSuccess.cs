using FluentAssertions;
using Messenger.BusinessLogic.ApiQueries.Auth;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiQueries.LoginCommandHandlerTests;

public class LoginTestSuccess : IntegrationTestBase, IIntegrationTest
{
	[Fact]
	public async Task Test()
	{
		await RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);

		var loginCommand = new LoginCommand(
			CommandHelper.Registration21ThCommand().Nickname,
			CommandHelper.Registration21ThCommand().Password);
		
		var loginResult = await RequestAsync(loginCommand, CancellationToken.None);

		loginResult.IsSuccess.Should().BeTrue();
	}
}