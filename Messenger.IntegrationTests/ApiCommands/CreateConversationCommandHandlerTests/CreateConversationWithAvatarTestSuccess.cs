using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.CreateConversationCommandHandlerTests;

public class CreateConversationWithAvatarTestSuccess : IntegrationTestBase, IIntegrationTest
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
        
        conversation.IsSuccess.Should().BeTrue();
        conversation.Value.IsOwner.Should().BeTrue();
        conversation.Value.IsMember.Should().BeTrue();
        conversation.Value.AvatarLink.Should().NotBeNull();

        var deleteChatCommand = new DeleteChatCommand(user21Th.Value.Id, conversation.Value.Id);

        await MessengerModule.RequestAsync(deleteChatCommand, CancellationToken.None);
    }
}