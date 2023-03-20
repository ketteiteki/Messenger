using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.CreateChannelCommandHandlerTests;

public class CreateChannelWithAvatarTestSuccess : IntegrationTestBase, IIntegrationTest
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
		
        var channel = await MessengerModule.RequestAsync(createConversationCommand, CancellationToken.None);

        channel.IsSuccess.Should().BeTrue();
        channel.Value.IsOwner.Should().BeTrue();
        channel.Value.IsMember.Should().BeTrue();
        channel.Value.AvatarLink.Should().NotBeNull();

        var deleteChatCommand = new DeleteChatCommand(user21Th.Value.Id, channel.Value.Id);

        await MessengerModule.RequestAsync(deleteChatCommand, CancellationToken.None);
    }
}