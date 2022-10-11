using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.JoinToConversationCommandHandlerTests;

public class JoinToConversationTestSuccess : IntegrationTestBase, IIntegrationTest
{
	[Fact]
	public async Task Test()
	{
		var user21th = await MessengerModule.RequestAsync(CommandHelper.Registration21thCommand(), CancellationToken.None);
		var alice = await MessengerModule.RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);
		
		var command = new CreateConversationCommand(
			RequestorId: user21th.Value.Id,
			Name: "convers",
			Title: "21ths den",
			AvatarFile: null);

		var conversation = await MessengerModule.RequestAsync(command, CancellationToken.None);
		
		var joinToConversationCommand = new JoinToConversationCommand(
			RequestorId: alice.Value.Id,
			ChatId: conversation.Value.Id);

		var joinToConversation = await MessengerModule.RequestAsync(joinToConversationCommand, CancellationToken.None);

		joinToConversation.IsSuccess.Should().BeTrue();
	}
}