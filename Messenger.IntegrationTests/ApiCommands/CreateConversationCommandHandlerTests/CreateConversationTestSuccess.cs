using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.CreateConversationCommandHandlerTests;

public class CreateConversationTestSuccess : IntegrationTestBase, IIntegrationTest
{
	[Fact]
	public async Task Test()
	{
		var user = await MessengerModule.RequestAsync(CommandHelper.Registration21thCommand(), CancellationToken.None);

		var command = new CreateConversationCommand(
			RequestorId: user.Value.Id,
			Name: "qwerty",
			Title: "qwery",
			AvatarFile: null);

		var conversation = await MessengerModule.RequestAsync(command, CancellationToken.None);
		
		conversation.IsSuccess.Should().BeTrue();
		conversation.Value.IsOwner.Should().BeTrue();
		conversation.Value.IsMember.Should().BeTrue();
	}
}