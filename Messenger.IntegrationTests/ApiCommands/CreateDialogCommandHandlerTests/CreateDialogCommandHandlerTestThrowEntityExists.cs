using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Dialogs;
using Messenger.BusinessLogic.Responses;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.CreateDialogCommandHandlerTests;

public class CreateDialogCommandHandlerTestThrowEntityExists : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
        var alice = await MessengerModule.RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);

        var createDialog21ThAliceCommand = new CreateDialogCommand(user21Th.Value.Id, alice.Value.Id);

        await MessengerModule.RequestAsync(createDialog21ThAliceCommand, CancellationToken.None);
        
        var secondCreateDialog21ThAliceResult = 
            await MessengerModule.RequestAsync(createDialog21ThAliceCommand, CancellationToken.None);

        secondCreateDialog21ThAliceResult.Error.Should().BeOfType<DbEntityExistsError>();
    }
}