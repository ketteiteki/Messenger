using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.BusinessLogic.ApiQueries.Users;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiQueries.GetUserListByChatQueryHandlerTests;

public class GetUserListByChatTestSuccess : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
        var alice = await MessengerModule.RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);
        var alex = await MessengerModule.RequestAsync(CommandHelper.RegistrationAlexCommand(), CancellationToken.None);
        var bob = await MessengerModule.RequestAsync(CommandHelper.RegistrationBobCommand(), CancellationToken.None);

        var conversation = await MessengerModule.RequestAsync(new CreateChatCommand(
            RequesterId: user21Th.Value.Id,
            Name: "qwerty",
            Title: "qwerty",
            Type: ChatType.Conversation,
            AvatarFile: null), CancellationToken.None);

        await MessengerModule.RequestAsync(new JoinToChatCommand(
            RequesterId: alice.Value.Id,
            ChatId: conversation.Value.Id), CancellationToken.None);
        
        await MessengerModule.RequestAsync(new JoinToChatCommand(
            RequesterId: alex.Value.Id,
            ChatId: conversation.Value.Id), CancellationToken.None);

        var createRoleUserAliceCommand = new CreateOrUpdateRoleUserInConversationCommand(
            RequesterId: user21Th.Value.Id,
            ChatId: conversation.Value.Id,
            UserId: alice.Value.Id,
            RoleTitle: "qwerty",
            RoleColor.Blue,
            CanBanUser: false,
            CanChangeChatData: false,
            CanAddAndRemoveUserToConversation: false,
            CanGivePermissionToUser: true);
        
        await MessengerModule.RequestAsync(createRoleUserAliceCommand, CancellationToken.None);

        var userListByChatResult = await MessengerModule.RequestAsync(new GetUserListByChatQuery(
            RequesterId: bob.Value.Id,
            ChatId: conversation.Value.Id,
            Limit: 40,
            Page: 1), CancellationToken.None);

        userListByChatResult.Value.Count.Should().Be(3);
    }
}