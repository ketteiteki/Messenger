using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.CreateChannelCommandHandlerTests;

public class CreateChannelTestSuccess : IntegrationTestBase, IIntegrationTest
{
	[Fact]
	public async Task Test()
	{
		var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
		
		var createChannelCommand = new CreateChatCommand(
			RequesterId: user21Th.Value.Id,
			Name: "qwerty",
			Title: "qwerty",
			Type: ChatType.Channel,
			AvatarFile: null);
		
		var channel = await MessengerModule.RequestAsync(createChannelCommand, CancellationToken.None);

		channel.IsSuccess.Should().BeTrue();
		channel.Value.IsOwner.Should().BeTrue();
		channel.Value.IsMember.Should().BeTrue();
	}
}
