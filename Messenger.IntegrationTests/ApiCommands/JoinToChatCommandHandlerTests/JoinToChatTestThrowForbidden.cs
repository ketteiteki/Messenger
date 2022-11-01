using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.JoinToChatCommandHandlerTests;

public class JoinToChatTestThrowForbidden : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
        var alice = await MessengerModule.RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);
		
        var createConversationCommand = new CreateChatCommand(
            RequesterId: user21Th.Value.Id,
            Name: "qwerty",
            Title: "qwerty",
            Type: ChatType.Conversation,
            AvatarFile: null);

        var conversation = await MessengerModule.RequestAsync(createConversationCommand, CancellationToken.None);
		
        var firstJoinToConversationCommand = new JoinToChatCommand(
            RequesterId: alice.Value.Id,
            ChatId: conversation.Value.Id);

        await MessengerModule.RequestAsync(firstJoinToConversationCommand, CancellationToken.None);

        var banUserAliceInConversationBy21ThCommand = new BanUserInConversationCommand(
            RequesterId: user21Th.Value.Id,
            ChatId: conversation.Value.Id,
            UserId: alice.Value.Id,
            DateTime.UtcNow.AddDays(2));

        await MessengerModule.RequestAsync(banUserAliceInConversationBy21ThCommand, CancellationToken.None);
        
        var secondJoinToConversationCommand = new JoinToChatCommand(
            RequesterId: alice.Value.Id,
            ChatId: conversation.Value.Id);

        var secondJoinToConversationResult = 
            await MessengerModule.RequestAsync(secondJoinToConversationCommand, CancellationToken.None);

        secondJoinToConversationResult.Error.Should().BeOfType<ForbiddenError>();
    }
}