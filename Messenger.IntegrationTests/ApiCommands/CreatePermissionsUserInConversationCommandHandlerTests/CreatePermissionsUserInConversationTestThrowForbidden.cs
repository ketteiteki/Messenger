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
            user21Th.Value.Id,
            Name: "qwerty",
            Title: "qwerty",
            ChatType.Conversation,
            AvatarFile: null);

        var createConversationResult = await MessengerModule.RequestAsync(createConversationCommand, CancellationToken.None);

        var addAliceInConversationBy21ThCommand = new AddUserToConversationCommand(
            user21Th.Value.Id,
            createConversationResult.Value.Id,
            alice.Value.Id);
        
        var addAlexInConversationBy21ThCommand = new AddUserToConversationCommand(
            user21Th.Value.Id,
            createConversationResult.Value.Id,
            alex.Value.Id);

        await MessengerModule.RequestAsync(addAliceInConversationBy21ThCommand, CancellationToken.None);
        await MessengerModule.RequestAsync(addAlexInConversationBy21ThCommand, CancellationToken.None);

        var createAlicePermissionInConversationByBobCommand = new CreatePermissionsUserInConversationCommand(
            bob.Value.Id,
            createConversationResult.Value.Id,
            alice.Value.Id,
            CanSendMedia: false,
            MuteMinutes: null);
        
        var createAlicePermissionInConversationByBobResult = 
            await MessengerModule.RequestAsync(createAlicePermissionInConversationByBobCommand, CancellationToken.None);
        
        var createAlexPermissionInConversationByAliceCommand = new CreatePermissionsUserInConversationCommand(
            alice.Value.Id,
            createConversationResult.Value.Id,
            alex.Value.Id,
            CanSendMedia: false,
            MuteMinutes: null);
        
        var createAlexPermissionInConversationByAliceResult = 
            await MessengerModule.RequestAsync(createAlexPermissionInConversationByAliceCommand, CancellationToken.None);

        var addBobInConversationBy21ThCommand = new AddUserToConversationCommand(
            user21Th.Value.Id,
            createConversationResult.Value.Id,
            bob.Value.Id);
        
        await MessengerModule.RequestAsync(addBobInConversationBy21ThCommand, CancellationToken.None);
        
        var createBobPermissionInConversationByAlexCommand = new CreatePermissionsUserInConversationCommand(
            alex.Value.Id,
            createConversationResult.Value.Id,
            alex.Value.Id,
            CanSendMedia: false,
            MuteMinutes: 10);

        var createBobPermissionInConversationByAlexResult = 
            await MessengerModule.RequestAsync(createBobPermissionInConversationByAlexCommand, CancellationToken.None);
        
        createAlicePermissionInConversationByBobResult.Error.Should().BeOfType<ForbiddenError>();
        createAlexPermissionInConversationByAliceResult.Error.Should().BeOfType<ForbiddenError>();
        createBobPermissionInConversationByAlexResult.Error.Should().BeOfType<ForbiddenError>();
    }
}