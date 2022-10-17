using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Channels;
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

		var command = new CreateChannelCommand(
			RequesterId: user21Th.Value.Id,
			Name: "convers",
			Title: "21ths den",
			AvatarFile: null);

		var channel = await MessengerModule.RequestAsync(command, CancellationToken.None);

		channel.IsSuccess.Should().BeTrue();
		channel.Value.IsOwner.Should().BeTrue();
		channel.Value.IsMember.Should().BeTrue();
	}
}
