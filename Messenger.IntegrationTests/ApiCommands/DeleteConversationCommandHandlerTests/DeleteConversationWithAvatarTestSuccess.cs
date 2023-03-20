using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.DeleteConversationCommandHandlerTests;

public class DeleteConversationWithAvatarTestSuccess : IntegrationTestBase, IIntegrationTest
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
            AvatarFile: FilesHelper.GetFile());

        var conversation = await MessengerModule.RequestAsync(createConversationCommand, CancellationToken.None);

        conversation.Value.AvatarLink.Should().NotBeNull();
        
        var deleteConversationCommand = new DeleteChatCommand(
            RequesterId: user21Th.Value.Id,
            ChatId: conversation.Value.Id);

        var deletedConversation = await MessengerModule.RequestAsync(deleteConversationCommand, CancellationToken.None);

        deletedConversation.IsSuccess.Should().BeTrue();
    }
}