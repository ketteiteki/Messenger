using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Enums;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.DeleteChannelCommandHandlerTests;

public class DeleteChannelTestThrowForbidden : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
        var alice = await RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);
        var bob = await RequestAsync(CommandHelper.RegistrationBobCommand(), CancellationToken.None);
        
        var createChannelCommand = new CreateChatCommand(
            user21Th.Value.Id,
            Name: "qwerty",
            Title: "qwerty",
            ChatType.Channel,
            AvatarFile: null);
		
        var channel = await RequestAsync(createChannelCommand, CancellationToken.None);

        var aliceJoinToConversationCommand = new JoinToChatCommand(alice.Value.Id, channel.Value.Id);
        
        await RequestAsync(aliceJoinToConversationCommand, CancellationToken.None);
        
        var deleteChannelByAliceCommand = new DeleteChatCommand(alice.Value.Id, channel.Value.Id);
		
        var deleteChannelByAliceResult = 
            await RequestAsync(deleteChannelByAliceCommand, CancellationToken.None);
        
        var deleteChannelByBobCommand = new DeleteChatCommand(bob.Value.Id, channel.Value.Id);
		
        var deleteChannelByBobResult = 
            await RequestAsync(deleteChannelByBobCommand, CancellationToken.None);

        deleteChannelByAliceResult.Error.Should().BeOfType<ForbiddenError>();
        deleteChannelByBobResult.Error.Should().BeOfType<ForbiddenError>();
    }
}