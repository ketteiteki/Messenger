using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Messages;
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
                FilesHelper.GetFile(),
                FilesHelper.GetFile()
            }), CancellationToken.None);

        var createSecondMessageByAliceResult = await MessengerModule.RequestAsync(new CreateMessageCommand(
            RequesterId: alice.Value.Id,
            Text: "qwerty422",
            ReplyToId: null,
            ChatId: conversation.Value.Id,
            Files: new FormFileCollection
            {
                FilesHelper.GetFile(),
                FilesHelper.GetFile()
            }), CancellationToken.None);

        createFirstMessageByAliceResult.Value.Attachments.Count.Should().Be(2);
            
        createSecondMessageByAliceResult.Value.Attachments.Count.Should().Be(2);
            
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
    }
}