using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Messages;
using Messenger.Domain.Enums;
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
            user21Th.Value.Id,
            Name: "qwerty",
            Title: "qwerty",
            ChatType.Conversation,
            AvatarFile: null);

        var createConversationResult = await MessengerModule.RequestAsync(createConversationCommand, CancellationToken.None);

        var aliceJoinToConversationCommand = new JoinToChatCommand(alice.Value.Id, createConversationResult.Value.Id); 
        var bobJoinToConversationCommand = new JoinToChatCommand(bob.Value.Id, createConversationResult.Value.Id); 
        
        await MessengerModule.RequestAsync(aliceJoinToConversationCommand, CancellationToken.None);
        await MessengerModule.RequestAsync(bobJoinToConversationCommand, CancellationToken.None);

        var firstCreateMessageByAliceCommand = new CreateMessageCommand(
            alice.Value.Id,
            Text: "qwerty2",
            ReplyToId: null,
            createConversationResult.Value.Id,
            new FormFileCollection
            {
                FilesHelper.GetFile(),
                FilesHelper.GetFile()
            });
        
        var firstCreateMessageByAliceResult = 
            await MessengerModule.RequestAsync(firstCreateMessageByAliceCommand, CancellationToken.None);

        var secondCreateMessageByAliceCommand = new CreateMessageCommand(
            alice.Value.Id,
            Text: "qwerty422",
            ReplyToId: null,
            createConversationResult.Value.Id,
            new FormFileCollection
            {
                FilesHelper.GetFile(),
                FilesHelper.GetFile()
            });
        
        var secondCreateMessageByAliceResult = 
            await MessengerModule.RequestAsync(secondCreateMessageByAliceCommand, CancellationToken.None);

        firstCreateMessageByAliceResult.Value.Attachments.Count.Should().Be(2);
        secondCreateMessageByAliceResult.Value.Attachments.Count.Should().Be(2);

        var deleteAliceMessageByBobCommand = new DeleteMessageCommand(
            bob.Value.Id,
            firstCreateMessageByAliceResult.Value.Id,
            IsDeleteForAll: true);
        
        var deleteAliceMessageByBobResult = 
            await MessengerModule.RequestAsync(deleteAliceMessageByBobCommand, CancellationToken.None);

        var deleteAliceMessageBy21ThCommand = new DeleteMessageCommand(
            user21Th.Value.Id,
            firstCreateMessageByAliceResult.Value.Id,
            IsDeleteForAll: true);
        
        var deleteAliceMessageBy21ThResult = 
            await MessengerModule.RequestAsync(deleteAliceMessageBy21ThCommand, CancellationToken.None);

        var deleteAliceMessageByAliceCommand = new DeleteMessageCommand(
            user21Th.Value.Id,
            secondCreateMessageByAliceResult.Value.Id,
            IsDeleteForAll: true);
        
        var deleteAliceMessageByAliceResult =
            await MessengerModule.RequestAsync(deleteAliceMessageByAliceCommand, CancellationToken.None);

        deleteAliceMessageByBobResult.IsSuccess.Should().BeFalse();

        deleteAliceMessageBy21ThResult.IsSuccess.Should().BeTrue();
            
        deleteAliceMessageByAliceResult.IsSuccess.Should().BeTrue();
    }
}