using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.Domain.Enum;
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

        await using var fileStream = new FileStream(Path.Combine(AppContext.BaseDirectory, "../../../Files/img1.jpg"), FileMode.Open);
        
        var createConversationCommand = new CreateChatCommand(
            RequesterId: user21Th.Value.Id,
            Name: "qwerty",
            Title: "qwerty",
            Type: ChatType.Conversation,
            AvatarFile: new FormFile(
                baseStream: fileStream,
                baseStreamOffset: 0,
                length: fileStream.Length,
                name: "qwerty",
                fileName: "qwerty.jpg"));
		
        var conversation = await MessengerModule.RequestAsync(createConversationCommand, CancellationToken.None);
        
        conversation.IsSuccess.Should().BeTrue();
        conversation.Value.IsOwner.Should().BeTrue();
        conversation.Value.IsMember.Should().BeTrue();
        conversation.Value.AvatarLink.Should().NotBeNull();

        var pathAvatar = Path.Combine(BaseDirService.GetPathWwwRoot(), conversation.Value.AvatarLink.Split("/")[^1]);
        
        File.Exists(pathAvatar).Should().BeTrue();
        
        File.Delete(pathAvatar);
    }
}