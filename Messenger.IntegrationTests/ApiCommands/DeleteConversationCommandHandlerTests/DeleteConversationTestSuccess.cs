using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiQueries.Chats;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.DeleteConversationCommandHandlerTests;

public class DeleteConversationTestSuccess : IntegrationTestBase, IIntegrationTest
{
	[Fact]
	public async Task Test()
	{
		var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);

		var createConversationCommand = new CreateChatCommand(
			RequesterId: user21Th.Value.Id,
			Name: "qwerty",
			Title: "qwerty",
			Type: ChatType.Conversation,
			AvatarFile: null);
		
		var conversation = await MessengerModule.RequestAsync(createConversationCommand, CancellationToken.None);

		var deleteConversationCommand = new DeleteChatCommand(
			RequesterId: user21Th.Value.Id,
			ChatId: conversation.Value.Id);
		
		await MessengerModule.RequestAsync(deleteConversationCommand, CancellationToken.None);

		var conversationForCheckCommand = new GetChatQuery(
			RequesterId: user21Th.Value.Id,
			ChatId: conversation.Value.Id);

		var conversationForCheck = await MessengerModule.RequestAsync(conversationForCheckCommand, CancellationToken.None);
		
		conversationForCheck.Value.Should().BeNull();
	}
}