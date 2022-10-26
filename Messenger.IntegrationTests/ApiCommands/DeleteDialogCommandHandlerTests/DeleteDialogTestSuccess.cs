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

        var dialog21ThAliceResult = await MessengerModule.RequestAsync(new CreateDialogCommand(
            RequesterId: user21Th.Value.Id,
            UserId: alice.Value.Id), CancellationToken.None);
        
        var dialogBobAlexResult = await MessengerModule.RequestAsync(new CreateDialogCommand(
            RequesterId: bob.Value.Id,
            UserId: alex.Value.Id), CancellationToken.None);

        var deletedDialog21ThAliceResult = await MessengerModule.RequestAsync(new DeleteDialogCommand(
            RequesterId: user21Th.Value.Id,
            ChatId: dialog21ThAliceResult.Value.Id,
            IsForBoth: false), CancellationToken.None);
        
        var deletedDialogBobAlexResult = await MessengerModule.RequestAsync(new DeleteDialogCommand(
            RequesterId: bob.Value.Id,
            ChatId: dialogBobAlexResult.Value.Id,
            IsForBoth: true), CancellationToken.None);

        deletedDialog21ThAliceResult.IsSuccess.Should().BeTrue();
        deletedDialogBobAlexResult.IsSuccess.Should().BeTrue();
    }
}