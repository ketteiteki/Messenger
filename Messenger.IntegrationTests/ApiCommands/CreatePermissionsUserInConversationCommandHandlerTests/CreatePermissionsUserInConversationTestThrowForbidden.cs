using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.CreatePermissionsUserInConversationCommandHandlerTests;

public class CreatePermissionsUserInConversationTestThrowForbidden : IntegrationTestBase, IIntegrationTest
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

        var addAliceInConversationBy21ThCommand = new AddUserToConversationCommand(
            RequesterId: user21Th.Value.Id,
            ChatId: conversation.Value.Id,
            UserId: alice.Value.Id);
        
        var addAlexInConversationBy21ThCommand = new AddUserToConversationCommand(
            RequesterId: user21Th.Value.Id,
            ChatId: conversation.Value.Id,
            UserId: alex.Value.Id);

        await MessengerModule.RequestAsync(addAliceInConversationBy21ThCommand, CancellationToken.None);
        await MessengerModule.RequestAsync(addAlexInConversationBy21ThCommand, CancellationToken.None);

        var createPermissionUserAliceInConversationByBobCommand = new CreatePermissionsUserInConversationCommand(
            RequesterId: bob.Value.Id,
            ChatId: conversation.Value.Id,
            UserId: alice.Value.Id,
            CanSendMedia: false);
        
        var createPermissionUserAliceInConversationByBobResult = 
            await MessengerModule.RequestAsync(createPermissionUserAliceInConversationByBobCommand, CancellationToken.None);
        
        var createPermissionUserAlexInConversationByAliceCommand = new CreatePermissionsUserInConversationCommand(
            RequesterId: alice.Value.Id,
            ChatId: conversation.Value.Id,
            UserId: alex.Value.Id,
            CanSendMedia: false);
        
        var createPermissionUserAlexInConversationByAliceResult = 
            await MessengerModule.RequestAsync(createPermissionUserAlexInConversationByAliceCommand, CancellationToken.None);

        createPermissionUserAliceInConversationByBobResult.Error.Should().BeOfType<ForbiddenError>();
        createPermissionUserAlexInConversationByAliceResult.Error.Should().BeOfType<ForbiddenError>();
    }
}