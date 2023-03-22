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

        var getDialogQuery = new GetDialogQuery(bob.Value.Id, alice.Value.Id);
        
        var getDialogResult = await MessengerModule.RequestAsync(getDialogQuery, CancellationToken.None);

        getDialogResult.Error.Should().BeOfType<DbEntityNotFoundError>();
    }
}