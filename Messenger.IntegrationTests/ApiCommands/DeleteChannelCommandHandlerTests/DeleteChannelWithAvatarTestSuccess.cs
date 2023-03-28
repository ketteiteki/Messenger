using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.Domain.Enums;
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
            user21Th.Value.Id,
            Name: "qwerty",
            Title: "qwerty",
            ChatType.Channel,
            AvatarFile: FilesHelper.GetFile());
		
        var createChannelResult = await MessengerModule.RequestAsync(createChannelCommand, CancellationToken.None);

        createChannelResult.Value.AvatarLink.Should().NotBeNull();
        
        var deleteChannelCommand = new DeleteChatCommand(user21Th.Value.Id, createChannelResult.Value.Id);
		
        var deletedChannelResult = await MessengerModule.RequestAsync(deleteChannelCommand, CancellationToken.None);

        deletedChannelResult.IsSuccess.Should().BeTrue();
    }
}