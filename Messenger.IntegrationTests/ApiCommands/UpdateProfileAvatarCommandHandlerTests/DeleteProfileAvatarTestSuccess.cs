using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Profiles;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.UpdateProfileAvatarCommandHandlerTests;

public class DeleteProfileAvatarTestSuccess : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);

        var firstUpdateProfileAvatarResult = await MessengerModule.RequestAsync(new UpdateProfileAvatarCommand(
            RequesterId: user21Th.Value.Id,
            AvatarFile: FilesHelper.GetFile()), CancellationToken.None);

        var secondUpdateProfileAvatarResult = await MessengerModule.RequestAsync(new UpdateProfileAvatarCommand(
            RequesterId: user21Th.Value.Id,
            AvatarFile: null), CancellationToken.None);

        secondUpdateProfileAvatarResult.IsSuccess.Should().BeTrue();

        File.Exists(Path.Combine(
            BaseDirService.GetPathWwwRoot(), firstUpdateProfileAvatarResult.Value.AvatarLink.Split("/")[^1]))
            .Should().BeFalse();
    }
}