using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.BusinessLogic.ApiQueries.Users;
using Messenger.Domain.Enums;
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

        var createConversationCommand = new CreateChatCommand(
            user21Th.Value.Id,
            Name: "qwerty",
            Title: "qwerty",
            ChatType.Conversation,
            AvatarFile: null);
        
        var createConversationResult = await MessengerModule.RequestAsync(createConversationCommand, CancellationToken.None);

        var aliceJoinConversationCommand = new JoinToChatCommand(alice.Value.Id, createConversationResult.Value.Id);
        var alexJoinConversationCommand = new JoinToChatCommand(alex.Value.Id, createConversationResult.Value.Id);
        
        await MessengerModule.RequestAsync(aliceJoinConversationCommand, CancellationToken.None);
        await MessengerModule.RequestAsync(alexJoinConversationCommand, CancellationToken.None);

        var createAliceRoleCommand = new CreateOrUpdateRoleUserInConversationCommand(
            user21Th.Value.Id,
            createConversationResult.Value.Id,
            alice.Value.Id,
            RoleTitle: "qwerty",
            RoleColor.Blue,
            CanBanUser: false,
            CanChangeChatData: false,
            CanAddAndRemoveUserToConversation: false,
            CanGivePermissionToUser: true);
        
        await MessengerModule.RequestAsync(createAliceRoleCommand, CancellationToken.None);

        var getUserListByChatQuery = new GetUserListByChatQuery(
            bob.Value.Id,
            createConversationResult.Value.Id,
            Limit: 40,
            Page: 1);
        
        var getUserListByChatResult = await MessengerModule.RequestAsync(getUserListByChatQuery, CancellationToken.None);

        getUserListByChatResult.Value.Count.Should().Be(3);
    }
}