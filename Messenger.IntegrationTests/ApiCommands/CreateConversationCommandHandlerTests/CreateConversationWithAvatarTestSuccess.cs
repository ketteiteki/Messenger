using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.BusinessLogic.Services;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.CreateConversationCommandHandlerTests;

public class CreateConversationWithAvatarTestSuccess : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);

        var fileStream = new FileStream(Path.Combine(AppContext.BaseDirectory, "../../../Files/img1.jpg"), FileMode.Open);
        
        var command = new CreateConversationCommand(
            RequesterId: user21Th.Value.Id,
            Name: "qwerty",
            Title: "qwerty",
            AvatarFile: new FormFile(
                baseStream: fileStream,
                baseStreamOffset: 0,
                length: fileStream.Length,
                name: "qwerty",
                fileName: "qwerty"));
            
        var conversation = await MessengerModule.RequestAsync(command, CancellationToken.None);
		
        conversation.IsSuccess.Should().BeTrue();
        conversation.Value.IsOwner.Should().BeTrue();
        conversation.Value.IsMember.Should().BeTrue();
        conversation.Value.AvatarLink.Should().NotBeNull();

        var pathAvatar = Path.Combine(BaseDirService.GetPathWwwRoot(), conversation.Value.AvatarLink.Split("/")[^1]);
        
        File.Exists(pathAvatar).Should().BeTrue();
        
        File.Delete(pathAvatar);
    }
}