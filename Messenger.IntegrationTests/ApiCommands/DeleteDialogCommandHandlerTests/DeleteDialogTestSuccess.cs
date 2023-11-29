using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Dialogs;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.DeleteDialogCommandHandlerTests;

public class DeleteDialogTestSuccess : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
        var alice = await RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);
        var bob = await RequestAsync(CommandHelper.RegistrationBobCommand(), CancellationToken.None);
        var alex = await RequestAsync(CommandHelper.RegistrationAlexCommand(), CancellationToken.None);

        var createDialogWithAliceBy21ThCommand = new CreateDialogCommand(user21Th.Value.Id, alice.Value.Id);
        var createDialogWithAlexByBobCommand = new CreateDialogCommand(bob.Value.Id, alex.Value.Id);
        
        var createDialogWithAliceBy21ThResult = 
            await RequestAsync(createDialogWithAliceBy21ThCommand, CancellationToken.None);
        
        var createDialogWithAlexByBobResult = 
            await RequestAsync(createDialogWithAlexByBobCommand, CancellationToken.None);

        var deleteDialogWithAliceBy21ThCommand = new DeleteDialogCommand(
            user21Th.Value.Id,
            createDialogWithAliceBy21ThResult.Value.Id,
            IsDeleteForAll: false);
        
        var deleteDialogWithAliceBy21ThResult = 
            await RequestAsync(deleteDialogWithAliceBy21ThCommand, CancellationToken.None);

        var deleteDialogWithAlexByBobCommand = new DeleteDialogCommand(
            bob.Value.Id,
            createDialogWithAlexByBobResult.Value.Id,
            IsDeleteForAll: true);
        
        var deleteDialogWithAlexByBobResult = 
            await RequestAsync(deleteDialogWithAlexByBobCommand, CancellationToken.None);

        deleteDialogWithAliceBy21ThResult.IsSuccess.Should().BeTrue();
        deleteDialogWithAlexByBobResult.IsSuccess.Should().BeTrue();
    }
}