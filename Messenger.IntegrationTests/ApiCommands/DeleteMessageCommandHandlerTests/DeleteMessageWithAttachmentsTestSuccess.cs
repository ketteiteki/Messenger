using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Messages;
using Messenger.BusinessLogic.Services;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.DeleteMessageCommandHandlerTests;

public class DeleteMessageWithAttachmentsTestSuccess : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
        var alice = await MessengerModule.RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);
        var bob = await MessengerModule.RequestAsync(CommandHelper.RegistrationBobCommand(), CancellationToken.None);

        await using var fileStream = 
            new FileStream(Path.Combine(AppContext.BaseDirectory, "../../../Files/img1.jpg"), FileMode.Open);
       
        var createConversationCommand = new CreateChatCommand(
            RequesterId: user21Th.Value.Id,
            Name: "qwerty",
            Title: "qwerty",
            Type: ChatType.Conversation,
            AvatarFile: null);

        var conversation = await MessengerModule.RequestAsync(createConversationCommand, CancellationToken.None);

        await MessengerModule.RequestAsync(new JoinToChatCommand(
            RequesterId: alice.Value.Id,
            ChatId: conversation.Value.Id), CancellationToken.None);
        
        await MessengerModule.RequestAsync(new JoinToChatCommand(
            RequesterId: bob.Value.Id,
            ChatId: conversation.Value.Id), CancellationToken.None);

        var createFirstMessageByAliceResult = await MessengerModule.RequestAsync(new CreateMessageCommand(
            RequesterId: alice.Value.Id,
            Text: "qwerty2",
            ReplyToId: null,
            ChatId: conversation.Value.Id,
            Files: new FormFileCollection
            {
                new FormFile(
                    baseStream: fileStream,
                    baseStreamOffset: 0,
                    length: fileStream.Length,
                    name: "qwerty",
                    fileName: "qwerty.jpg"),
                new FormFile(
                    baseStream: fileStream,
                    baseStreamOffset: 0,
                    length: fileStream.Length,
                    name: "qwerty",
                    fileName: "qwerty.jpg")
            }), CancellationToken.None);

        var createSecondMessageByAliceResult = await MessengerModule.RequestAsync(new CreateMessageCommand(
            RequesterId: alice.Value.Id,
            Text: "qwerty422",
            ReplyToId: null,
            ChatId: conversation.Value.Id,
            Files: new FormFileCollection
            {
                new FormFile(
                    baseStream: fileStream,
                    baseStreamOffset: 0,
                    length: fileStream.Length,
                    name: "qwerty",
                    fileName: "qwerty.jpg"),
                new FormFile(
                    baseStream: fileStream,
                    baseStreamOffset: 0,
                    length: fileStream.Length,
                    name: "qwerty",
                    fileName: "qwerty.jpg")
            }), CancellationToken.None);

        foreach (var attachment in createFirstMessageByAliceResult.Value.Attachments)
        {
            var pathAvatar = Path.Combine(BaseDirService.GetPathWwwRoot(), attachment.Link.Split("/")[^1]);
            
            File.Exists(pathAvatar).Should().BeTrue();
        }
            
        foreach (var attachment in createSecondMessageByAliceResult.Value.Attachments)
        {
            var pathAvatar = Path.Combine(BaseDirService.GetPathWwwRoot(), attachment.Link.Split("/")[^1]);
            
            File.Exists(pathAvatar).Should().BeTrue();
        }
            
        var deleteMessageAliceByBobResult = await MessengerModule.RequestAsync(new DeleteMessageCommand(
            RequesterId: bob.Value.Id,
            MessageId: createFirstMessageByAliceResult.Value.Id,
            IsDeleteForAll: true), CancellationToken.None);
            
        var deleteMessageAliceBy21ThResult = await MessengerModule.RequestAsync(new DeleteMessageCommand(
            RequesterId: user21Th.Value.Id,
            MessageId: createFirstMessageByAliceResult.Value.Id,
            IsDeleteForAll: true), CancellationToken.None);
            
        var deleteMessageAliceByAliceResult = await MessengerModule.RequestAsync(new DeleteMessageCommand(
            RequesterId: user21Th.Value.Id,
            MessageId: createSecondMessageByAliceResult.Value.Id,
            IsDeleteForAll: true), CancellationToken.None);

        deleteMessageAliceByBobResult.IsSuccess.Should().BeFalse();

        deleteMessageAliceBy21ThResult.IsSuccess.Should().BeTrue();
            
        deleteMessageAliceByAliceResult.IsSuccess.Should().BeTrue();
            
        foreach (var attachment in createFirstMessageByAliceResult.Value.Attachments)
        {
            var pathAvatar = Path.Combine(BaseDirService.GetPathWwwRoot(), attachment.Link.Split("/")[^1]);
            
            File.Exists(pathAvatar).Should().BeFalse();
        }
            
        foreach (var attachment in createSecondMessageByAliceResult.Value.Attachments)
        {
            var pathAvatar = Path.Combine(BaseDirService.GetPathWwwRoot(), attachment.Link.Split("/")[^1]);
            
            File.Exists(pathAvatar).Should().BeFalse();
        }
    }
}