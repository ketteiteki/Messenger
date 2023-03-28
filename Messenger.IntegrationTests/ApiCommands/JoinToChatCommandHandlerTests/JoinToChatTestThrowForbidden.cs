using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Enums;
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
            user21Th.Value.Id,
            Name: "qwerty",
            Title: "qwerty",
            ChatType.Conversation,
            AvatarFile: null);

        var createConversationResult = await MessengerModule.RequestAsync(createConversationCommand, CancellationToken.None);

        var firstAliceJoinToConversationCommand = new JoinToChatCommand(alice.Value.Id,createConversationResult.Value.Id);

        await MessengerModule.RequestAsync(firstAliceJoinToConversationCommand, CancellationToken.None);

        var banAliceInConversationBy21ThCommand = new BanUserInConversationCommand(
            user21Th.Value.Id,
            createConversationResult.Value.Id,
            alice.Value.Id,
            BanMinutes: 15);

        await MessengerModule.RequestAsync(banAliceInConversationBy21ThCommand, CancellationToken.None);
        
        var secondAliceJoinToConversationCommand = new JoinToChatCommand(alice.Value.Id, createConversationResult.Value.Id);

        var secondAliceJoinToConversationResult = 
            await MessengerModule.RequestAsync(secondAliceJoinToConversationCommand, CancellationToken.None);

        secondAliceJoinToConversationResult.Error.Should().BeOfType<ForbiddenError>();
    }
}