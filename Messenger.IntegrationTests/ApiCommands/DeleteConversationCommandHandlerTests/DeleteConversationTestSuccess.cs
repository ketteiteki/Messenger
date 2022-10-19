using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.BusinessLogic.ApiQueries.Conversations;
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

		var command = new CreateConversationCommand(
			RequesterId: user21Th.Value.Id,
			Name: "convers",
			Title: "21ths den",
			AvatarFile: null);

		var conversation = await MessengerModule.RequestAsync(command, CancellationToken.None);
		
		var deleteConversationCommand = new DeleteConversationCommand(
			RequesterId: user21Th.Value.Id,
			ChatId: conversation.Value.Id);

		await MessengerModule.RequestAsync(deleteConversationCommand, CancellationToken.None);

		var conversationForCheckCommand = new GetConversationQuery(
			RequesterId: user21Th.Value.Id,
			ChatId: conversation.Value.Id);

		var conversationForCheck = await MessengerModule.RequestAsync(conversationForCheckCommand, CancellationToken.None);
		
		conversationForCheck.Value.Should().BeNull();
	}
}