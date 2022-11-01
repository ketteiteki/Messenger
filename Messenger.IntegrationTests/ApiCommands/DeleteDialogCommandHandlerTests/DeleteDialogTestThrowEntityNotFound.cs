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
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
        var alice = await MessengerModule.RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);
        var bob = await MessengerModule.RequestAsync(CommandHelper.RegistrationBobCommand(), CancellationToken.None);
        
        var dialog21ThAliceResult = await MessengerModule.RequestAsync(new CreateDialogCommand(
            RequesterId: user21Th.Value.Id,
            UserId: alice.Value.Id), CancellationToken.None);
        
        var deletedDialog21ThAliceByBobResult = await MessengerModule.RequestAsync(new DeleteDialogCommand(
            RequesterId: bob.Value.Id,
            ChatId: dialog21ThAliceResult.Value.Id,
            IsDeleteForAll: true), CancellationToken.None);

        deletedDialog21ThAliceByBobResult.Error.Should().BeOfType<DbEntityNotFoundError>();
    }
}