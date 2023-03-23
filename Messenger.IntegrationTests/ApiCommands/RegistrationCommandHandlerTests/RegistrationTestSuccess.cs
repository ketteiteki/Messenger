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
			CommandHelper.Registration21ThCommand().DisplayName,
			CommandHelper.Registration21ThCommand().Nickname,
			CommandHelper.Registration21ThCommand().Password,
			UserAgent: "Mozilla",
			Ip: "323.432.21.542");

		var registrationResult = await MessengerModule.RequestAsync(registrationCommand, CancellationToken.None);

		registrationResult.IsSuccess.Should().BeTrue();
	}
}