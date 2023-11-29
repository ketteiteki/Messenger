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
        var user21Th = await RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
        var alice = await RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);

        var createDialogBetween21ThAndAliceCommand = new CreateDialogCommand(user21Th.Value.Id, alice.Value.Id); 
        
        await RequestAsync(createDialogBetween21ThAndAliceCommand, CancellationToken.None);

        var getDialogBetween21ThAndAliceBy21ThQuery = new GetDialogQuery(user21Th.Value.Id, alice.Value.Id);
        var getDialogBetween21ThAndAliceByAliceQuery = new GetDialogQuery(alice.Value.Id, user21Th.Value.Id);

        var getDialogBetween21ThAndAliceBy21ThResult =
            await RequestAsync(getDialogBetween21ThAndAliceBy21ThQuery, CancellationToken.None);
        
        var getDialogBetween21ThAndAliceByAliceResult = 
            await RequestAsync(getDialogBetween21ThAndAliceByAliceQuery, CancellationToken.None);

        getDialogBetween21ThAndAliceBy21ThResult.Value.Members.Count.Should().Be(2);
        getDialogBetween21ThAndAliceBy21ThResult.Value.Members
            .First(m => m.Id != user21Th.Value.Id).Id.Should().Be(alice.Value.Id);
        getDialogBetween21ThAndAliceBy21ThResult.Value.Members
            .First(m => m.Id != user21Th.Value.Id).Nickname.Should().Be(alice.Value.Nickname);
        getDialogBetween21ThAndAliceBy21ThResult.Value.Members
            .First(m => m.Id != user21Th.Value.Id).DisplayName.Should().Be(alice.Value.DisplayName);
        getDialogBetween21ThAndAliceBy21ThResult.Value.Members
            .First(m => m.Id != user21Th.Value.Id).Bio.Should().Be(alice.Value.Bio);
        
        getDialogBetween21ThAndAliceByAliceResult.Value.Members.Count.Should().Be(2);
        getDialogBetween21ThAndAliceByAliceResult.Value.Members
            .First(m => m.Id != alice.Value.Id).Id.Should().Be(user21Th.Value.Id);
        getDialogBetween21ThAndAliceByAliceResult.Value.Members
            .First(m => m.Id != alice.Value.Id).Nickname.Should().Be(user21Th.Value.Nickname);
        getDialogBetween21ThAndAliceByAliceResult.Value.Members
            .First(m => m.Id != alice.Value.Id).DisplayName.Should().Be(user21Th.Value.DisplayName);
        getDialogBetween21ThAndAliceByAliceResult.Value.Members
            .First(m => m.Id != alice.Value.Id).Bio.Should().Be(user21Th.Value.Bio);
    }
}