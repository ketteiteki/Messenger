using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiQueries.Chats;
using Messenger.Domain.Enums;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.DeleteConversationCommandHandlerTests;

public class DeleteConversationTestSuccess : IntegrationTestBase, IIntegrationTest
{
	[Fact]
	public async Task Test()
	{
		var user21Th = await RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);

		var createConversationCommand = new CreateChatCommand(
			user21Th.Value.Id,
			Name: "qwerty",
			Title: "qwerty",
			ChatType.Conversation,
			AvatarFile: null);
		
		var createConversationResult = await RequestAsync(createConversationCommand, CancellationToken.None);

		var deleteConversationCommand = new DeleteChatCommand(user21Th.Value.Id, createConversationResult.Value.Id);
		
		await RequestAsync(deleteConversationCommand, CancellationToken.None);

		var getConversationCommand = new GetChatQuery(user21Th.Value.Id, createConversationResult.Value.Id);

		var getConversationResult = await RequestAsync(getConversationCommand, CancellationToken.None);
		
		getConversationResult.Value.Should().BeNull();
	}
}