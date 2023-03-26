using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.Domain.Enums;
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
            user21Th.Value.Id,
            Name: "qwerty",
            Title: "qwerty",
            ChatType.Conversation,
            AvatarFile: FilesHelper.GetFile());
		
        var createConversationResult = await MessengerModule.RequestAsync(createConversationCommand, CancellationToken.None);
        
        createConversationResult.IsSuccess.Should().BeTrue();
        createConversationResult.Value.IsOwner.Should().BeTrue();
        createConversationResult.Value.IsMember.Should().BeTrue();
        createConversationResult.Value.AvatarLink.Should().NotBeNull();

        var avatarFileName = createConversationResult.Value.AvatarLink.Split("/")[^1];

        await BlobService.DeleteBlobAsync(avatarFileName);
    }
}