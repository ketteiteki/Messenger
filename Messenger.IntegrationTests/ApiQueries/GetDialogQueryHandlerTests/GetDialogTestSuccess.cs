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
        
        var getDialogBy21ThResult = await MessengerModule.RequestAsync(new GetDialogQuery(
            RequesterId: user21Th.Value.Id,
            UserId: alice.Value.Id), CancellationToken.None);
        
        var getDialogByAliceResult = await MessengerModule.RequestAsync(new GetDialogQuery(
            RequesterId: alice.Value.Id,
            UserId: user21Th.Value.Id), CancellationToken.None);

        getDialogBy21ThResult.Value.Members.Count.Should().Be(2);
        getDialogBy21ThResult.Value.Members.First(m => m.Id != user21Th.Value.Id).Id.Should().Be(alice.Value.Id);
        getDialogBy21ThResult.Value.Members.First(m => m.Id != user21Th.Value.Id).Nickname.Should().Be(alice.Value.NickName);
        getDialogBy21ThResult.Value.Members.First(m => m.Id != user21Th.Value.Id).DisplayName.Should().Be(alice.Value.DisplayName);
        getDialogBy21ThResult.Value.Members.First(m => m.Id != user21Th.Value.Id).Bio.Should().Be(alice.Value.Bio);
        
        getDialogByAliceResult.Value.Members.Count.Should().Be(2);
        getDialogByAliceResult.Value.Members.First(m => m.Id != alice.Value.Id).Id.Should().Be(user21Th.Value.Id);
        getDialogByAliceResult.Value.Members.First(m => m.Id != alice.Value.Id).Nickname.Should().Be(user21Th.Value.NickName);
        getDialogByAliceResult.Value.Members.First(m => m.Id != alice.Value.Id).DisplayName.Should().Be(user21Th.Value.DisplayName);
        getDialogByAliceResult.Value.Members.First(m => m.Id != alice.Value.Id).Bio.Should().Be(user21Th.Value.Bio);
    }
}