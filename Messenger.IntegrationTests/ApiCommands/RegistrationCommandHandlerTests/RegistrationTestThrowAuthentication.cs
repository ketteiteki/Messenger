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
            DisplayName: CommandHelper.Registration21ThCommand().DisplayName,
            Nickname: CommandHelper.Registration21ThCommand().Nickname,
            Password: CommandHelper.Registration21ThCommand().Password,
            UserAgent: "Mozilla",
            Ip: "323.432.21.542");

        await MessengerModule.RequestAsync(firstRegistrationCommand, CancellationToken.None);
        
        var secondRegistrationCommand = new RegistrationCommand(
            DisplayName: CommandHelper.Registration21ThCommand().DisplayName,
            Nickname: CommandHelper.Registration21ThCommand().Nickname,
            Password: CommandHelper.Registration21ThCommand().Password,
            UserAgent: "Mozilla",
            Ip: "428.764.324.653");

        var secondRegistrationResult = 
            await MessengerModule.RequestAsync(secondRegistrationCommand, CancellationToken.None);

        secondRegistrationResult.Error.Should().BeOfType<AuthenticationError>();
    }
}