using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.BusinessLogic.ApiQueries.Conversations;
using Messenger.Domain.Enums;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiQueries.GetUserPermissionsQueryHandlerTests;

public class GetUserPermissionsTestSuccess : IntegrationTestBase, IIntegrationTest
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
        
        var aliceJoinConversationCommand = new JoinToChatCommand(alice.Value.Id, createConversationResult.Value.Id);
        await MessengerModule.RequestAsync(aliceJoinConversationCommand, CancellationToken.None);
        
        var createPermissionsUserInConversationCommand =
            new CreatePermissionsUserInConversationCommand(
                user21Th.Value.Id,
                createConversationResult.Value.Id,
                alice.Value.Id,
                CanSendMedia: false,
                MuteMinutes: 50);
        
        await MessengerModule.RequestAsync(createPermissionsUserInConversationCommand, CancellationToken.None);

        var getUserPermissionsQuery = new GetUserPermissionsQuery(createConversationResult.Value.Id, alice.Value.Id);

        var getUserPermissionsResult = await MessengerModule.RequestAsync(getUserPermissionsQuery, CancellationToken.None);

        getUserPermissionsResult.Value.UserId.Should().Be(alice.Value.Id);
        getUserPermissionsResult.Value.ChatId.Should().Be(createConversationResult.Value.Id);
        getUserPermissionsResult.Value.CanSendMedia.Should().Be(createPermissionsUserInConversationCommand.CanSendMedia);
        getUserPermissionsResult.Value.MuteDateOfExpire.Should().NotBeNull();
    }
}