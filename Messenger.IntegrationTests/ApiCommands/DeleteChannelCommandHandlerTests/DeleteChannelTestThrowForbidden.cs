using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.DeleteChannelCommandHandlerTests;

public class DeleteChannelTestThrowForbidden : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
        var alice = await MessengerModule.RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);
        var bob = await MessengerModule.RequestAsync(CommandHelper.RegistrationBobCommand(), CancellationToken.None);
        
        var createChannelCommand = new CreateChatCommand(
            user21Th.Value.Id,
            Name: "qwerty",
            Title: "qwerty",
            ChatType.Channel,
            AvatarFile: null);
		
        var channel = await MessengerModule.RequestAsync(createChannelCommand, CancellationToken.None);

        var aliceJoinToConversationCommand = new JoinToChatCommand(alice.Value.Id, channel.Value.Id);
        
        await MessengerModule.RequestAsync(aliceJoinToConversationCommand, CancellationToken.None);
        
        var deleteChannelByAliceCommand = new DeleteChatCommand(alice.Value.Id, channel.Value.Id);
		
        var deleteChannelByAliceResult = 
            await MessengerModule.RequestAsync(deleteChannelByAliceCommand, CancellationToken.None);
        
        var deleteChannelByBobCommand = new DeleteChatCommand(bob.Value.Id, channel.Value.Id);
		
        var deleteChannelByBobResult = 
            await MessengerModule.RequestAsync(deleteChannelByBobCommand, CancellationToken.None);

        deleteChannelByAliceResult.Error.Should().BeOfType<ForbiddenError>();
        deleteChannelByBobResult.Error.Should().BeOfType<ForbiddenError>();
    }
}