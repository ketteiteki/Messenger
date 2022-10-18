using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.BusinessLogic.Services;
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

        using (var fileStream = new FileStream(Path.Combine(AppContext.BaseDirectory, "../../../Files/img1.jpg"),
                   FileMode.Open))
        {
            var command = new CreateConversationCommand(
                RequesterId: user21Th.Value.Id,
                Name: "convers",
                Title: "21ths den",
                AvatarFile: new FormFile(
                    baseStream: fileStream,
                    baseStreamOffset: 0,
                    length: fileStream.Length,
                    name: "qwerty",
                    fileName: "qwerty"));

            var conversation = await MessengerModule.RequestAsync(command, CancellationToken.None);
		
            var pathAvatar = Path.Combine(BaseDirService.GetPathWwwRoot(), conversation.Value.AvatarLink.Split("/")[^1]);
        
            File.Exists(pathAvatar).Should().BeTrue();
        
            var deleteConversationCommand = new DeleteConversationCommand(
                RequesterId: user21Th.Value.Id,
                ChatId: conversation.Value.Id);

            var deletedConversation = await MessengerModule.RequestAsync(deleteConversationCommand, CancellationToken.None);

            deletedConversation.IsSuccess.Should().BeTrue();
        
            File.Exists(pathAvatar).Should().BeFalse();
        }
    }
}