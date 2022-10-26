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

        var createDialogResult = await MessengerModule.RequestAsync(new CreateDialogCommand(
            RequesterId: user21Th.Value.Id,
            UserId: alice.Value.Id), CancellationToken.None);

        var getDialogResult = await MessengerModule.RequestAsync(new GetChatQuery(
            RequesterId: user21Th.Value.Id,
            ChatId: createDialogResult.Value.Id), CancellationToken.None);

        getDialogResult.Value.MembersCount.Should().Be(2);
        getDialogResult.Value.Members.Count.Should().Be(2);
        getDialogResult.Value.Members.First(m => m.Id == user21Th.Value.Id).Nickname.Should().Be(user21Th.Value.NickName);
        getDialogResult.Value.Members.First(m => m.Id == alice.Value.Id).Nickname.Should().Be(alice.Value.NickName);
    }
}