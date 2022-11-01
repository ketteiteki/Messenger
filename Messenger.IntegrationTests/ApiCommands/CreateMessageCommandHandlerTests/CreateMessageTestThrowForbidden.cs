using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Messages;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.CreateMessageCommandHandlerTests;

public class CreateMessageTestThrowForbidden : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
        var bob = await MessengerModule.RequestAsync(CommandHelper.RegistrationBobCommand(), CancellationToken.None);

        var createConversationCommand = new CreateChatCommand(
            RequesterId: user21Th.Value.Id,
            Name: "qwerty",
            Title: "qwerty",
            Type: ChatType.Conversation,
            AvatarFile: null);
		
        var conversation = await MessengerModule.RequestAsync(createConversationCommand, CancellationToken.None);
        
        var createdMessageByBobCommand = new CreateMessageCommand(
            RequesterId: bob.Value.Id,
            Text: "qwerty3",
            ReplyToId: null,
            ChatId: conversation.Value.Id,
            Files: null);
        
        var createMessageByBobResult = await MessengerModule.RequestAsync(createdMessageByBobCommand, CancellationToken.None);
        
        createMessageByBobResult.Error.Should().BeOfType<ForbiddenError>();
    }
}