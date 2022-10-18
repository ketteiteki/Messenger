using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Channels;
using Messenger.BusinessLogic.Services;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.CreateChannelCommandHandlerTests;

public class CreateChannelWithAvatarTestSuccess : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);

        var fileStream = new FileStream(Path.Combine(AppContext.BaseDirectory, "../../../Files/img1.jpg"), FileMode.Open);
        
        var command = new CreateChannelCommand(
            RequesterId: user21Th.Value.Id,
            Name: "convers",
            Title: "21ths den",
            AvatarFile: new FormFile(
                baseStream: fileStream,
                baseStreamOffset: 0,
                length: fileStream.Length,
                name: "qwerty",
                fileName: "qwerty"));

        var channel = await MessengerModule.RequestAsync(command, CancellationToken.None);

        channel.IsSuccess.Should().BeTrue();
        channel.Value.IsOwner.Should().BeTrue();
        channel.Value.IsMember.Should().BeTrue();
        channel.Value.AvatarLink.Should().NotBeNull();

        var pathAvatar = Path.Combine(BaseDirService.GetPathWwwRoot(), channel.Value.AvatarLink.Split("/")[^1]);
        
        File.Exists(pathAvatar).Should().BeTrue();
        
        File.Delete(pathAvatar);
    }
}