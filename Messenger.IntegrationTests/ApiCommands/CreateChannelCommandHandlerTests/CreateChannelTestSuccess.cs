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
		var requester = EntityHelper.CreateUser21th();

		DatabaseContextFixture.Users.Add(requester);
		await DatabaseContextFixture.SaveChangesAsync();

		var command = new CreateChannelCommand(
			RequestorId: requester.Id,
			Name: "convers",
			Title: "21ths den",
			AvatarFile: null);

		var result = await MessengerModule.RequestAsync(command, CancellationToken.None);
	}
}
