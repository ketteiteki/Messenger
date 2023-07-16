using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Dialogs;
using Messenger.BusinessLogic.Responses;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.DeleteDialogCommandHandlerTests;

public class DeleteDialogTestThrowEntityNotFound : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
        var alice = await RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);
        var bob = await RequestAsync(CommandHelper.RegistrationBobCommand(), CancellationToken.None);

        var createDialogWithAliceBy21ThCommand = new CreateDialogCommand(user21Th.Value.Id, alice.Value.Id);
        
        var createDialogWithAliceBy21ThResult = 
            await RequestAsync(createDialogWithAliceBy21ThCommand, CancellationToken.None);

        var deleteDialogWithAliceAnd21ThByBobCommand = new DeleteDialogCommand(
            bob.Value.Id,
            createDialogWithAliceBy21ThResult.Value.Id,
            IsDeleteForAll: true);
        
        var deleteDialogWithAliceAnd21ThByBobResult = 
            await RequestAsync(deleteDialogWithAliceAnd21ThByBobCommand, CancellationToken.None);

        deleteDialogWithAliceAnd21ThByBobResult.Error.Should().BeOfType<DbEntityNotFoundError>();
    }
}