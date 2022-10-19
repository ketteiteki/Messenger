using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Profiles;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.DeleteProfileCommandHandlerTests;

public class DeleteProfileTestSuccess : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);

        var deletedProfile = await MessengerModule.RequestAsync(new DeleteProfileCommand(
            RequesterId: user21Th.Value.Id), CancellationToken.None);

        deletedProfile.IsSuccess.Should().BeTrue();
    }
}