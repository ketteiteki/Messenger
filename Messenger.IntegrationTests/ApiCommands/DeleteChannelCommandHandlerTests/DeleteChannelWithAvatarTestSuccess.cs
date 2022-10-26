using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.Services;
using Messenger.Domain.Enum;
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

        await using var fileStream = new FileStream(Path.Combine(AppContext.BaseDirectory, "../../../Files/img1.jpg"),
            FileMode.Open);
        
        var createChannelCommand = new CreateChatCommand(
            RequesterId: user21Th.Value.Id,
            Name: "qwerty",
            Title: "qwerty",
            Type: ChatType.Channel,
            AvatarFile: new FormFile(
                baseStream: fileStream,
                baseStreamOffset: 0,
                length: fileStream.Length,
                name: "qwerty",
                fileName: "qwerty"));
		
        var channel = await MessengerModule.RequestAsync(createChannelCommand, CancellationToken.None);

        var pathAvatar = Path.Combine(BaseDirService.GetPathWwwRoot(), channel.Value.AvatarLink.Split("/")[^1]);
        
        File.Exists(pathAvatar).Should().BeTrue();
        
        var deleteChannelCommand = new DeleteChatCommand(
            RequesterId: user21Th.Value.Id,
            ChatId: channel.Value.Id);
		
        var deletedChannel = await MessengerModule.RequestAsync(deleteChannelCommand, CancellationToken.None);

        deletedChannel.IsSuccess.Should().BeTrue();
        
        File.Exists(pathAvatar).Should().BeFalse();
    }
}