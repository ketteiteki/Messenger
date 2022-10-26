using FluentAssertions;
using Messenger.BusinessLogic.ApiQueries.Dialogs;
using Messenger.BusinessLogic.Responses;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiQueries.GetChatQueryHandlerTests;

public class GetDialogTestThrowEntityNotFound : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var alice = await MessengerModule.RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);
        var bob = await MessengerModule.RequestAsync(CommandHelper.RegistrationBobCommand(), CancellationToken.None);

        var getDialogQuery = await MessengerModule.RequestAsync(new GetDialogQuery(
            RequesterId: bob.Value.Id,
            UserId: alice.Value.Id), CancellationToken.None);

        getDialogQuery.Error.Should().BeOfType<DbEntityNotFoundError>();
    }
}