using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Dialogs;
using Messenger.BusinessLogic.ApiQueries.Chats;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiQueries.GetChatQueryHandlerTests;

public class GetDialogTestSuccess : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
        var alice = await MessengerModule.RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);

        var createDialogCommand = new CreateDialogCommand(user21Th.Value.Id, alice.Value.Id);
        
        var createDialogResult = await MessengerModule.RequestAsync(createDialogCommand, CancellationToken.None);

        var getDialogQuery = new GetChatQuery(user21Th.Value.Id, createDialogResult.Value.Id);
        
        var getDialogResult = await MessengerModule.RequestAsync(getDialogQuery, CancellationToken.None);

        getDialogResult.Value.MembersCount.Should().Be(2);
        getDialogResult.Value.Members.Count.Should().Be(2);
        getDialogResult.Value.Members.First(m => m.Id == user21Th.Value.Id).Nickname.Should().Be(user21Th.Value.NickName);
        getDialogResult.Value.Members.First(m => m.Id == alice.Value.Id).Nickname.Should().Be(alice.Value.NickName);
    }
}