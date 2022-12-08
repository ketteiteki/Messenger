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
            RequesterId: user21Th.Value.Id,
            Name: "qwerty",
            Title: "qwerty",
            Type: ChatType.Conversation,
            AvatarFile: null);
		
        var conversation = await MessengerModule.RequestAsync(createConversationCommand, CancellationToken.None);
        
        await MessengerModule.RequestAsync(
            new JoinToChatCommand(
                RequesterId: alice.Value.Id,
                ChatId: conversation.Value.Id), CancellationToken.None);
        
        await MessengerModule.RequestAsync(
            new JoinToChatCommand(
                RequesterId: alex.Value.Id,
                ChatId: conversation.Value.Id), CancellationToken.None);

        var banUserAliceInConversationByBobCommand = new BanUserInConversationCommand(
            RequesterId: bob.Value.Id,
            ChatId: conversation.Value.Id,
            UserId: alice.Value.Id,
            BanMinutes: 15);

        var banUserAliceInConversationByBobResult =
            await MessengerModule.RequestAsync(banUserAliceInConversationByBobCommand, CancellationToken.None);
        
        var banUserAlexInConversationByAliceCommand = new BanUserInConversationCommand(
            RequesterId: bob.Value.Id,
            ChatId: conversation.Value.Id,
            UserId: alice.Value.Id,
            BanMinutes: 15);

        var banUserAlexInConversationByAliceResult =
            await MessengerModule.RequestAsync(banUserAlexInConversationByAliceCommand, CancellationToken.None);

        banUserAliceInConversationByBobResult.Error.Should().BeOfType<ForbiddenError>();
        banUserAlexInConversationByAliceResult.Error.Should().BeOfType<ForbiddenError>();
    }
}