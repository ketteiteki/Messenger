using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.CreateOrUpdateRoleUserInConversationCommandHandlerTests;

public class CreateRoleUserInConversationTestThrowForbidden : IntegrationTestBase, IIntegrationTest
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
            RequesterId: alex.Value.Id,
            ChatId: conversation.Value.Id), CancellationToken.None);

        var createRoleUserAlexInConversationByAliceCommand = new CreateOrUpdateRoleUserInConversationCommand(
            RequesterId: alice.Value.Id,
            ChatId: conversation.Value.Id,
            UserId: alex.Value.Id,
            RoleTitle: "moderator",
            RoleColor: RoleColor.Cyan,
            CanBanUser: true,
            CanChangeChatData: false,
            CanAddAndRemoveUserToConversation: true,
            CanGivePermissionToUser: false);

        var createRoleUserAlexInConversationByAliceResult =
            await MessengerModule.RequestAsync(createRoleUserAlexInConversationByAliceCommand, CancellationToken.None);
        
        var createRoleUserAliceInConversationByBobCommand = new CreateOrUpdateRoleUserInConversationCommand(
            RequesterId: bob.Value.Id,
            ChatId: conversation.Value.Id,
            UserId: alice.Value.Id,
            RoleTitle: "moderator",
            RoleColor: RoleColor.Cyan,
            CanBanUser: true,
            CanChangeChatData: false,
            CanAddAndRemoveUserToConversation: true,
            CanGivePermissionToUser: false);

        var createRoleUserAliceInConversationByBobResult =
            await MessengerModule.RequestAsync(createRoleUserAliceInConversationByBobCommand, CancellationToken.None);

        createRoleUserAlexInConversationByAliceResult.Error.Should().BeOfType<ForbiddenError>();
        createRoleUserAliceInConversationByBobResult.Error.Should().BeOfType<ForbiddenError>();
    }
}