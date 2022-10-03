using Messenger.BusinessLogic.Channels.Command;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.Channels.CommandsTests;

public class CreateChannelCommandHandlerTestSuccess : IntegrationTestBase, IIntegrationTest
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

		await MessengerModule.RequestAsync(command, CancellationToken.None);
	}
}
