using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Enum;
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
            RequesterId: user21Th.Value.Id,
            Name: "qwerty",
            Title: "qwerty",
            Type: ChatType.Conversation,
            AvatarFile: null);

        var conversation = await MessengerModule.RequestAsync(createConversationCommand, CancellationToken.None);
		
        await MessengerModule.RequestAsync(new JoinToChatCommand(
            RequesterId: alice.Value.Id,
            ChatId: conversation.Value.Id), CancellationToken.None);
        
        await MessengerModule.RequestAsync(new JoinToChatCommand(
            RequesterId: bob.Value.Id,
            ChatId: conversation.Value.Id), CancellationToken.None);

        var createRoleUserAliceInConversationBy21ThCommand = new CreateOrUpdateRoleUserInConversationCommand(
            RequesterId: user21Th.Value.Id,
            ChatId: conversation.Value.Id,
            UserId: alice.Value.Id,
            RoleTitle: "moderator",
            RoleColor: RoleColor.Cyan,
            CanBanUser: true,
            CanChangeChatData: false,
            CanAddAndRemoveUserToConversation: false,
            CanGivePermissionToUser:false);

        await MessengerModule.RequestAsync(createRoleUserAliceInConversationBy21ThCommand, CancellationToken.None);

        var removeRoleUserAliceInConversationByBobCommand = new RemoveRoleUserInConversationCommand(
            RequesterId: bob.Value.Id,
            ChatId: conversation.Value.Id,
            UserId: alice.Value.Id);

        var removeRoleUserAliceInConversationByBobResult = 
            await MessengerModule.RequestAsync(removeRoleUserAliceInConversationByBobCommand, CancellationToken.None);
        
        var removeRoleUserAliceInConversationByAlexCommand = new RemoveRoleUserInConversationCommand(
            RequesterId: alex.Value.Id,
            ChatId: conversation.Value.Id,
            UserId: alice.Value.Id);

        var removeRoleUserAliceInConversationByAlexResult = 
            await MessengerModule.RequestAsync(removeRoleUserAliceInConversationByAlexCommand, CancellationToken.None);

        removeRoleUserAliceInConversationByBobResult.Error.Should().BeOfType<ForbiddenError>();
        removeRoleUserAliceInConversationByAlexResult.Error.Should().BeOfType<ForbiddenError>();
    }
}