using Messenger.BusinessLogic.ApiCommands.Channels;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.DeleteChannelCommandHandlerTests;

public class DeleteChannelTestSuccess : IntegrationTestBase, IIntegrationTest
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

		var channel = await MessengerModule.RequestAsync(command, CancellationToken.None);

		await MessengerModule.RequestAsync(
			new DeleteChannelCommand(
				RequestorId: requester.Id, 
				ChannelId: channel.Id), CancellationToken.None);
	}
}