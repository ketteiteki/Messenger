using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.BanUserInConversationCommandHandlerTests;

public class BanUserInConversationTestThrowForbidden : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
        var alice = await MessengerModule.RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);
        var bob = await MessengerModule.RequestAsync(CommandHelper.RegistrationBobCommand(), CancellationToken.None);
        var alex = await MessengerModule.RequestAsync(CommandHelper.RegistrationAlexCommand(), CancellationToken.None);

        var createConversationCommand = new CreateChatCommand(
            user21Th.Value.Id,
            Name: "qwerty",
            Title: "qwerty",
            ChatType.Conversation,
            AvatarFile: null);
		
        var createConversationResult = await MessengerModule.RequestAsync(createConversationCommand, CancellationToken.None);

        var userAliceJoinToConversation = new JoinToChatCommand(alice.Value.Id, createConversationResult.Value.Id);
        var userAlexJoinToConversation = new JoinToChatCommand(alex.Value.Id, createConversationResult.Value.Id);
        
        await MessengerModule.RequestAsync(userAliceJoinToConversation, CancellationToken.None);
        await MessengerModule.RequestAsync(userAlexJoinToConversation, CancellationToken.None);

        var banUserAliceInConversationByBobCommand = new BanUserInConversationCommand(
            bob.Value.Id,
            createConversationResult.Value.Id,
            alice.Value.Id,
            BanMinutes: 15);
        
        var banUserAlexInConversationByAliceCommand = new BanUserInConversationCommand(
            bob.Value.Id,
            createConversationResult.Value.Id,
            alice.Value.Id,
            BanMinutes: 15);
        
        var banUserAliceInConversationByBobResult =
            await MessengerModule.RequestAsync(banUserAliceInConversationByBobCommand, CancellationToken.None);
        var banUserAlexInConversationByAliceResult =
            await MessengerModule.RequestAsync(banUserAlexInConversationByAliceCommand, CancellationToken.None);

        banUserAliceInConversationByBobResult.Error.Should().BeOfType<ForbiddenError>();
        banUserAlexInConversationByAliceResult.Error.Should().BeOfType<ForbiddenError>();
    }
}