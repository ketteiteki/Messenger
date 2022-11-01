using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.LeaveFromConversationCommandHandlerTests;

public class LeaveFromConversationTestThrowForbidden : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
        var alice = await MessengerModule.RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);
		
        var createConversationCommand = new CreateChatCommand(
            RequesterId: user21Th.Value.Id,
            Name: "qwerty",
            Title: "qwerty",
            Type: ChatType.Conversation,
            AvatarFile: null);

        var conversation = await MessengerModule.RequestAsync(createConversationCommand, CancellationToken.None);

        var leaveFromConversationResult = await MessengerModule.RequestAsync(new LeaveFromChatCommand(
            RequesterId: alice.Value.Id,
            ChatId: conversation.Value.Id), CancellationToken.None);

        leaveFromConversationResult.Error.Should().BeOfType<ForbiddenError>();
    }
}