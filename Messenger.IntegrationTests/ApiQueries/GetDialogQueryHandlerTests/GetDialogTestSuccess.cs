using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Dialogs;
using Messenger.BusinessLogic.ApiQueries.Dialogs;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiQueries.GetDialogQueryHandlerTests;

public class GetDialogTestSuccess : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
        var alice = await MessengerModule.RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);

        await MessengerModule.RequestAsync(new CreateDialogCommand(
            RequesterId: user21Th.Value.Id,
            UserId: alice.Value.Id), CancellationToken.None);
        
        var dialogFor21Th = await MessengerModule.RequestAsync(new GetDialogQuery(
            RequesterId: user21Th.Value.Id,
            WithWhomId: alice.Value.Id), CancellationToken.None);
        
        var dialogForAlice = await MessengerModule.RequestAsync(new GetDialogQuery(
            RequesterId: alice.Value.Id,
            WithWhomId: user21Th.Value.Id), CancellationToken.None);

        dialogFor21Th.Value.Members.Count.Should().Be(2);
        dialogFor21Th.Value.Members.First(m => m.Id != user21Th.Value.Id).Id.Should().Be(alice.Value.Id);
        dialogFor21Th.Value.Members.First(m => m.Id != user21Th.Value.Id).Nickname.Should().Be(alice.Value.Nickname);
        dialogFor21Th.Value.Members.First(m => m.Id != user21Th.Value.Id).DisplayName.Should().Be(alice.Value.DisplayName);
        dialogFor21Th.Value.Members.First(m => m.Id != user21Th.Value.Id).Bio.Should().Be(alice.Value.Bio);
        
        dialogForAlice.Value.Members.Count.Should().Be(2);
        dialogForAlice.Value.Members.First(m => m.Id != alice.Value.Id).Id.Should().Be(user21Th.Value.Id);
        dialogForAlice.Value.Members.First(m => m.Id != alice.Value.Id).Nickname.Should().Be(user21Th.Value.Nickname);
        dialogForAlice.Value.Members.First(m => m.Id != alice.Value.Id).DisplayName.Should().Be(user21Th.Value.DisplayName);
        dialogForAlice.Value.Members.First(m => m.Id != alice.Value.Id).Bio.Should().Be(user21Th.Value.Bio);
    }
}