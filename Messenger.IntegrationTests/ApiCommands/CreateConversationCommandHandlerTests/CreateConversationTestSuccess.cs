using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.CreateConversationCommandHandlerTests;

public class CreateConversationTestSuccess : IntegrationTestBase, IIntegrationTest
{
	[Fact]
	public async Task Test()
	{
		var user = EntityHelper.CreateUser21th();

		DatabaseContextFixture.Users.Add(user);
		await DatabaseContextFixture.SaveChangesAsync();

		var conversation = new CreateConversationCommand(
			RequestorId: user.Id,
			Name: "qwerty",
			Title: "qwery",
			AvatarFile: null);

		await MessengerModule.RequestAsync(conversation, CancellationToken.None);
	}
}