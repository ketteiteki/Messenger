using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Enums;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.RemoveRoleUserInConversationCommandHandlerTests;

public class RemoveRoleUserInConversationTestThrowForbidden : IntegrationTestBase, IIntegrationTest
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

        var aliceJoinChatCommand = new JoinToChatCommand(alice.Value.Id, createConversationResult.Value.Id);
        var bobJoinChatCommand = new JoinToChatCommand(bob.Value.Id, createConversationResult.Value.Id);
        
        await MessengerModule.RequestAsync(aliceJoinChatCommand, CancellationToken.None);
        await MessengerModule.RequestAsync(bobJoinChatCommand, CancellationToken.None);

        var createAliceRoleInConversationBy21ThCommand = new CreateOrUpdateRoleUserInConversationCommand(
            user21Th.Value.Id,
            createConversationResult.Value.Id,
            alice.Value.Id,
            RoleTitle: "moderator",
            RoleColor.Cyan,
            CanBanUser: true,
            CanChangeChatData: false,
            CanAddAndRemoveUserToConversation: false,
            CanGivePermissionToUser:false);

        await MessengerModule.RequestAsync(createAliceRoleInConversationBy21ThCommand, CancellationToken.None);

        var removeAliceRoleInConversationByBobCommand = new RemoveRoleUserInConversationCommand(
            bob.Value.Id,
            createConversationResult.Value.Id,
            alice.Value.Id);

        var removeAliceRoleInConversationByBobResult = 
            await MessengerModule.RequestAsync(removeAliceRoleInConversationByBobCommand, CancellationToken.None);
        
        var removeAliceRoleInConversationByAlexCommand = new RemoveRoleUserInConversationCommand(
            alex.Value.Id,
            createConversationResult.Value.Id,
            alice.Value.Id);

        var removeAliceRoleInConversationByAlexResult = 
            await MessengerModule.RequestAsync(removeAliceRoleInConversationByAlexCommand, CancellationToken.None);

        removeAliceRoleInConversationByBobResult.Error.Should().BeOfType<ForbiddenError>();
        removeAliceRoleInConversationByAlexResult.Error.Should().BeOfType<ForbiddenError>();
    }
}