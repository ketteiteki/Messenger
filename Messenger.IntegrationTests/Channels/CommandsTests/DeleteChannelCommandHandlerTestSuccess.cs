using Messenger.BusinessLogic.Channels.Command;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.Channels.CommandsTests;

public class DeleteChannelCommandHandlerTestSuccess : IntegrationTestBase, IIntegrationTest
{
	[Fact]
	public async Task Test()
	{
		var requester = EntityHelper.CreateUser21th();

		DatabaseContextFixture.Users.Add(requester);
		await DatabaseContextFixture.SaveChangesAsync();

		var command = new CreateChannelCommand
		{
			RequesterId = requester.Id,
			Name = "convers",
			Title = "21ths den"
		};

		var channel = await MessengerModule.RequestAsync(command, CancellationToken.None);

		await MessengerModule.RequestAsync(
			new DeleteChannelCommand {RequesterId = requester.Id, ChannelId = channel.Id}, CancellationToken.None);
	}
}