using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Messages;
using Messenger.Domain.Enum;
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

        var createConversationCommand = new CreateChatCommand(
            RequesterId: user21Th.Value.Id,
            Name: "qwerty",
            Title: "qwerty",
            Type: ChatType.Conversation,
            AvatarFile: null);
		
        var conversation = await MessengerModule.RequestAsync(createConversationCommand, CancellationToken.None);

        var createMessageBy21ThCommand = new CreateMessageCommand(
            RequesterId: user21Th.Value.Id,
            Text: "qwerty1",
            ReplyToId: null,
            ChatId: conversation.Value.Id,
            Files: new FormFileCollection
            {
                FilesHelper.GetFile(),
                FilesHelper.GetFile()
            });

        var createMessageBy21ThResult = await MessengerModule.RequestAsync(createMessageBy21ThCommand, CancellationToken.None);

        createMessageBy21ThResult.Value.Attachments.Count.Should().Be(2);
        
        var deleteMessageCommand = new DeleteMessageCommand(
            user21Th.Value.Id,
            createMessageBy21ThResult.Value.Id, 
            IsDeleteForAll: true);

        await MessengerModule.RequestAsync(deleteMessageCommand, CancellationToken.None);
    }
}