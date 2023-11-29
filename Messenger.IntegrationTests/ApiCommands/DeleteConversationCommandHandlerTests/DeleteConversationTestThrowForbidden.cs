using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Enums;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.DeleteConversationCommandHandlerTests;

public class DeleteConversationTestThrowForbidden : IntegrationTestBase, IIntegrationTest
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
            ChatType.Conversation,
            AvatarFile: null);
		
        var createConversationResult = await RequestAsync(createChannelCommand, CancellationToken.None);

        var aliceJoinToChannelCommand = new JoinToChatCommand(alice.Value.Id, createConversationResult.Value.Id);
        
        await RequestAsync(aliceJoinToChannelCommand, CancellationToken.None);
        
        var deleteConversationByAliceCommand = new DeleteChatCommand(alice.Value.Id, createConversationResult.Value.Id);
		
        var deleteConversationByAliceResult = 
            await RequestAsync(deleteConversationByAliceCommand, CancellationToken.None);
        
        var deleteConversationByBobCommand = new DeleteChatCommand(bob.Value.Id, createConversationResult.Value.Id);
		
        var deleteConversationByBobResult = 
            await RequestAsync(deleteConversationByBobCommand, CancellationToken.None);

        deleteConversationByAliceResult.Error.Should().BeOfType<ForbiddenError>();
        deleteConversationByBobResult.Error.Should().BeOfType<ForbiddenError>();
    }
}