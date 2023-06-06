using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Auth;
using Messenger.BusinessLogic.Responses;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.RegistrationCommandHandlerTests;

public class RegistrationTestThrowAuthentication : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var firstRegistrationCommand = new RegistrationCommand(
            CommandHelper.Registration21ThCommand().DisplayName,
            CommandHelper.Registration21ThCommand().Nickname,
            CommandHelper.Registration21ThCommand().Password);
        
        await MessengerModule.RequestAsync(firstRegistrationCommand, CancellationToken.None);
        
        var secondRegistrationCommand = new RegistrationCommand(
            CommandHelper.Registration21ThCommand().DisplayName,
            CommandHelper.Registration21ThCommand().Nickname,
            CommandHelper.Registration21ThCommand().Password);
        
        var secondRegistrationResult = 
            await MessengerModule.RequestAsync(secondRegistrationCommand, CancellationToken.None);

        secondRegistrationResult.Error.Should().BeOfType<AuthenticationError>();
    }
}