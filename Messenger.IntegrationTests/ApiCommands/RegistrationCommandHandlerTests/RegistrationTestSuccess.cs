using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Auth;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.RegistrationCommandHandlerTests;

public class RegistrationTestSuccess : IntegrationTestBase 
{
	[Fact]
	public async Task Test()
	{
		var registrationCommand = new RegistrationCommand(
			DisplayName: CommandHelper.Registration21ThCommand().DisplayName,
			Nickname: CommandHelper.Registration21ThCommand().Nickname,
			Password: CommandHelper.Registration21ThCommand().Password);

		var registrationHandler = await MessengerModule.RequestAsync(registrationCommand, CancellationToken.None);

		registrationHandler.IsSuccess.Should().BeTrue();
	}
}