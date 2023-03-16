using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.DeleteConversationCommandHandlerTests;

public class DeleteConversationWithAvatarTestSuccess : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);

        await using var fileStream = new FileStream(Path.Combine(AppContext.BaseDirectory, "../../../Files/img1.jpg"),
            FileMode.Open);
        
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
		
        var pathAvatar = Path.Combine(BaseDirService.GetPathWwwRoot(), conversation.Value.AvatarLink.Split("/")[^1]);
        
        File.Exists(pathAvatar).Should().BeTrue();
        
        var deleteConversationCommand = new DeleteChatCommand(
            RequesterId: user21Th.Value.Id,
            ChatId: conversation.Value.Id);

        var deletedConversation = await MessengerModule.RequestAsync(deleteConversationCommand, CancellationToken.None);

        deletedConversation.IsSuccess.Should().BeTrue();
        
        File.Exists(pathAvatar).Should().BeFalse();
    }
}