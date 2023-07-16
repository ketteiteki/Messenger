using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.Domain.Enums;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.DeleteConversationCommandHandlerTests;

public class DeleteConversationWithAvatarTestSuccess : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);

        var createConversationCommand = new CreateChatCommand(
            user21Th.Value.Id,
            Name: "qwerty",
            Title: "qwerty",
            ChatType.Conversation,
            FilesHelper.GetFile());

        var createConversationResult = await RequestAsync(createConversationCommand, CancellationToken.None);

        createConversationResult.Value.AvatarLink.Should().NotBeNull();
        
        var deleteConversationCommand = new DeleteChatCommand(user21Th.Value.Id, createConversationResult.Value.Id);

        var deletedConversationResult = await RequestAsync(deleteConversationCommand, CancellationToken.None);

        deletedConversationResult.IsSuccess.Should().BeTrue();
    }
}