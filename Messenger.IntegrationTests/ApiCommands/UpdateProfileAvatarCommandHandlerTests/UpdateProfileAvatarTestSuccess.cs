using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Profiles;
using Messenger.BusinessLogic.Services;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.UpdateProfileAvatarCommandHandlerTests;

public class UpdateProfileAvatarTestSuccess : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);

        await using var fileStream = new FileStream(
            Path.Combine(AppContext.BaseDirectory, "../../../Files/img1.jpg"), FileMode.Open);
        
        var userAfterUpdateAvatarResult = await MessengerModule.RequestAsync(new UpdateProfileAvatarCommand(
            RequesterId: user21Th.Value.Id,
            AvatarFile: new FormFile(
                baseStream: fileStream,
                baseStreamOffset: 0,
                length: fileStream.Length,
                name: "qwerty",
                fileName: "qwerty")), CancellationToken.None);
        
        var pathAvatar = Path.Combine(BaseDirService.GetPathWwwRoot(), userAfterUpdateAvatarResult.Value.AvatarLink.Split("/")[^1]);
        
        File.Exists(pathAvatar).Should().BeTrue();
        
        File.Delete(pathAvatar);
    }
}