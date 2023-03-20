using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.DeleteChannelCommandHandlerTests;

public class DeleteChannelWithAvatarTestSuccess : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);

        var createChannelCommand = new CreateChatCommand(
            RequesterId: user21Th.Value.Id,
            Name: "qwerty",
            Title: "qwerty",
            Type: ChatType.Channel,
            AvatarFile: FilesHelper.GetFile());
		
        var channel = await MessengerModule.RequestAsync(createChannelCommand, CancellationToken.None);

        channel.Value.AvatarLink.Should().NotBeNull();
        
        var deleteChannelCommand = new DeleteChatCommand(
            RequesterId: user21Th.Value.Id,
            ChatId: channel.Value.Id);
		
        var deletedChannel = await MessengerModule.RequestAsync(deleteChannelCommand, CancellationToken.None);

        deletedChannel.IsSuccess.Should().BeTrue();
    }
}