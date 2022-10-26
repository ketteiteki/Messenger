using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.LeaveFromConversationCommandHandlerTests;

public class LeaveFromConversationTestSuccess : IntegrationTestBase, IIntegrationTest
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
		
		var joinToConversationCommand = new JoinToChatCommand(
			RequesterId: alice.Value.Id,
			ChatId: conversation.Value.Id);

		await MessengerModule.RequestAsync(joinToConversationCommand, CancellationToken.None);
		
		var leaveFromConversationCommand = new LeaveFromChatCommand(
			RequesterId: alice.Value.Id,
			ChatId: conversation.Value.Id);

		var conversationAfterLeave = await MessengerModule.RequestAsync(leaveFromConversationCommand, CancellationToken.None);

		conversationAfterLeave.IsSuccess.Should().BeTrue();
	}
}