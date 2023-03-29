using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.Domain.Enums;
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
			user21Th.Value.Id,
			Name: "qwerty",
			Title: "qwerty",
			ChatType.Conversation,
			AvatarFile: null);

		var createConversationResult = await MessengerModule.RequestAsync(createConversationCommand, CancellationToken.None);
		
		var aliceJoinAToConversationCommand = new JoinToChatCommand(alice.Value.Id, createConversationResult.Value.Id);

		await MessengerModule.RequestAsync(aliceJoinAToConversationCommand, CancellationToken.None);
		
		var aliceLeaveConversationCommand = new LeaveFromChatCommand(alice.Value.Id, createConversationResult.Value.Id);

		var aliceLeaveConversationResult = await MessengerModule.RequestAsync(aliceLeaveConversationCommand, CancellationToken.None);

		aliceLeaveConversationResult.IsSuccess.Should().BeTrue();
	}
}