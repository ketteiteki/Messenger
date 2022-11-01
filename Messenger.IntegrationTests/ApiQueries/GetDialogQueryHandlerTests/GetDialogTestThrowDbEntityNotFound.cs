using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Dialogs;
using Messenger.BusinessLogic.ApiQueries.Chats;
using Messenger.BusinessLogic.Responses;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiQueries.GetDialogQueryHandlerTests;

public class GetDialogTestThrowDbEntityNotFound : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
        var alice = await MessengerModule.RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);
        var bob = await MessengerModule.RequestAsync(CommandHelper.RegistrationBobCommand(), CancellationToken.None);

        var createDialogResult = await MessengerModule.RequestAsync(new CreateDialogCommand(
            RequesterId: user21Th.Value.Id,
            UserId: alice.Value.Id), CancellationToken.None);

        var getDialogResult = await MessengerModule.RequestAsync(new GetChatQuery(
            RequesterId: bob.Value.Id,
            ChatId: createDialogResult.Value.Id), CancellationToken.None);

        getDialogResult.Error.Should().BeOfType<DbEntityNotFoundError>();
    }
}