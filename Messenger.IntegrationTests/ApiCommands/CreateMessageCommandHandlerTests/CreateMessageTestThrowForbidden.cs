using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Messages;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Enums;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.CreateMessageCommandHandlerTests;

public class CreateMessageTestThrowForbidden : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
        var bob = await RequestAsync(CommandHelper.RegistrationBobCommand(), CancellationToken.None);

        var createConversationCommand = new CreateChatCommand(
            user21Th.Value.Id,
            Name: "qwerty",
            Title: "qwerty",
            ChatType.Conversation,
            AvatarFile: null);
		
        var createConversationResult = await RequestAsync(createConversationCommand, CancellationToken.None);
        
        var createdMessageByBobCommand = new CreateMessageCommand(
            bob.Value.Id,
            Text: "qwerty3",
            ReplyToId: null,
            createConversationResult.Value.Id,
            Files: null);
        
        var createMessageByBobResult = await RequestAsync(createdMessageByBobCommand, CancellationToken.None);
        
        createMessageByBobResult.Error.Should().BeOfType<ForbiddenError>();
    }
}