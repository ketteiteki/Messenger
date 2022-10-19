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
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
        var alice = await MessengerModule.RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);
        var bob = await MessengerModule.RequestAsync(CommandHelper.RegistrationBobCommand(), CancellationToken.None);
        var alex = await MessengerModule.RequestAsync(CommandHelper.RegistrationAlexCommand(), CancellationToken.None);

        var dialog21ThAlice = await MessengerModule.RequestAsync(new CreateDialogCommand(
            RequesterId: user21Th.Value.Id,
            UserId: alice.Value.Id), CancellationToken.None);
        
        var dialogBobAlex = await MessengerModule.RequestAsync(new CreateDialogCommand(
            RequesterId: bob.Value.Id,
            UserId: alex.Value.Id), CancellationToken.None);

        var deletedDialog21ThAlice = await MessengerModule.RequestAsync(new DeleteDialogCommand(
            RequesterId: user21Th.Value.Id,
            ChatId: dialog21ThAlice.Value.Id,
            IsForBoth: false), CancellationToken.None);
        
        var deletedDialogBobAlex = await MessengerModule.RequestAsync(new DeleteDialogCommand(
            RequesterId: bob.Value.Id,
            ChatId: dialogBobAlex.Value.Id,
            IsForBoth: true), CancellationToken.None);

        deletedDialog21ThAlice.IsSuccess.Should().BeTrue();
        deletedDialogBobAlex.IsSuccess.Should().BeTrue();
    }
}