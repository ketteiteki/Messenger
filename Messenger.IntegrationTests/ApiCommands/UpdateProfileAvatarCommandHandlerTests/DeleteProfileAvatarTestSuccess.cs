using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Profiles;
using Messenger.BusinessLogic.Services;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.UpdateProfileAvatarCommandHandlerTests;

public class DeleteProfileAvatarTestSuccess : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);

        await using var fileStream = new FileStream(
            Path.Combine(AppContext.BaseDirectory, "../../../Files/img1.jpg"), FileMode.Open);

        var firstUpdateProfileAvatarResult = await MessengerModule.RequestAsync(new UpdateProfileAvatarCommand(
            RequesterId: user21Th.Value.Id,
            AvatarFile: new FormFile(
                baseStream: fileStream,
                baseStreamOffset: 0,
                length: fileStream.Length,
                name: "qwerty",
                fileName: "qwerty.jpg")), CancellationToken.None);

        var secondUpdateProfileAvatarResult = await MessengerModule.RequestAsync(new UpdateProfileAvatarCommand(
            RequesterId: user21Th.Value.Id,
            AvatarFile: null), CancellationToken.None);

        secondUpdateProfileAvatarResult.IsSuccess.Should().BeTrue();

        File.Exists(Path.Combine(
            BaseDirService.GetPathWwwRoot(), firstUpdateProfileAvatarResult.Value.AvatarLink.Split("/")[^1]))
            .Should().BeFalse();
    }
}