using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Profiles;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.UpdateProfileAvatarCommandHandlerTests;

public class UpdateProfileAvatarTestSuccess : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);

        var userAfterUpdateAvatarResult = await MessengerModule.RequestAsync(new UpdateProfileAvatarCommand(
            RequesterId: user21Th.Value.Id,
            AvatarFile: FilesHelper.GetFile()), CancellationToken.None);

        userAfterUpdateAvatarResult.IsSuccess.Should().BeTrue();
    }
}