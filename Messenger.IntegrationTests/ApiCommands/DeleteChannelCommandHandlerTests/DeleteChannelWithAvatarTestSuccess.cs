using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Channels;
using Messenger.BusinessLogic.Services;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.DeleteChannelCommandHandlerTests;

public class DeleteChannelWithAvatarTestSuccess : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);

        using (var fileStream = new FileStream(Path.Combine(AppContext.BaseDirectory, "../../../Files/img1.jpg"),
                   FileMode.Open))
        {
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

            var pathAvatar = Path.Combine(BaseDirService.GetPathWwwRoot(), channel.Value.AvatarLink.Split("/")[^1]);
        
            File.Exists(pathAvatar).Should().BeTrue();
        
            var deleteChannelCommand = new DeleteChannelCommand(
                RequesterId: user21Th.Value.Id,
                ChannelId: channel.Value.Id);
		
            var deletedChannel = await MessengerModule.RequestAsync(deleteChannelCommand, CancellationToken.None);

            deletedChannel.IsSuccess.Should().BeTrue();
        
            File.Exists(pathAvatar).Should().BeFalse();
        }
    }
}