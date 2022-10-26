using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Profiles;
using Messenger.BusinessLogic.Responses;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.DeleteProfileAvatarCommandHandlerTests;

public class DeleteProfileAvatarTestThrowForbidden : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);

        var deleteProfileAvatarResult = await MessengerModule.RequestAsync(new DeleteProfileAvatarCommand(
            RequesterId: user21Th.Value.Id), CancellationToken.None);

        deleteProfileAvatarResult.Error.Should().BeOfType<ForbiddenError>();
    }
}