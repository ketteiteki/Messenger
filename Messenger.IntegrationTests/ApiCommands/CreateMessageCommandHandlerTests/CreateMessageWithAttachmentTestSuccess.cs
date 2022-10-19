using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.BusinessLogic.ApiCommands.Messages;
using Messenger.BusinessLogic.Services;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.CreateMessageCommandHandlerTests;

public class CreateMessageWithAttachmentTestSuccess : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);

        await using var fileStream = new FileStream(Path.Combine(AppContext.BaseDirectory, "../../../Files/img1.jpg"), FileMode.Open);
        
        var conversation = await MessengerModule.RequestAsync(new CreateConversationCommand(
            RequesterId: user21Th.Value.Id,
            Name: "qwerty",
            Title: "qwerty",
            AvatarFile: null), CancellationToken.None);

        var createdMessageBy21ThCommand = new CreateMessageCommand(
            RequesterId: user21Th.Value.Id,
            Text: "qwerty1",
            ReplyToId: null,
            ChatId: conversation.Value.Id,
            Files: new FormFileCollection
            {
                new FormFile(
                    baseStream: fileStream,
                    baseStreamOffset: 0,
                    length: fileStream.Length,
                    name: "qwerty",
                    fileName: "qwerty"),
                new FormFile(
                    baseStream: fileStream,
                    baseStreamOffset: 0,
                    length: fileStream.Length,
                    name: "qwerty",
                    fileName: "qwerty")
            });

        var createdMessageBy21Th = await MessengerModule.RequestAsync(createdMessageBy21ThCommand, CancellationToken.None);

        foreach (var attachment in createdMessageBy21Th.Value.Attachments)
        {
            var pathAvatar = Path.Combine(BaseDirService.GetPathWwwRoot(), attachment.Link.Split("/")[^1]);
        
            File.Exists(pathAvatar).Should().BeTrue();
        
            File.Delete(pathAvatar);
        }
    }
}