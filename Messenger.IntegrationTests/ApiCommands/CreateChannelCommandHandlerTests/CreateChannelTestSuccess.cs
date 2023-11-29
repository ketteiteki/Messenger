using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.Domain.Enums;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.CreateChannelCommandHandlerTests;

public class CreateChannelTestSuccess : IntegrationTestBase, IIntegrationTest
{
	[Fact]
	public async Task Test()
	{
		var user21Th = await RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
		
		var createChannelCommand = new CreateChatCommand(
			user21Th.Value.Id,
			Name: "qwerty",
			Title: "qwerty",
			ChatType.Channel,
			AvatarFile: null);
		
		var createChannelResult = await RequestAsync(createChannelCommand, CancellationToken.None);

		createChannelResult.IsSuccess.Should().BeTrue();
		createChannelResult.Value.IsOwner.Should().BeTrue();
		createChannelResult.Value.IsMember.Should().BeTrue();
	}
}
