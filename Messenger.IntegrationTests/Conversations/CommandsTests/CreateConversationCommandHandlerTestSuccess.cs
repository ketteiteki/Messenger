using Messenger.BusinessLogic.Conversations.Commands;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.Conversations.CommandsTests;

public class CreateConversationCommandHandlerTestSuccess : IntegrationTestBase, IIntegrationTest
{
	[Fact]
	public async Task Test()
	{
		var user = EntityHelper.CreateUser21th();

		DatabaseContextFixture.Users.Add(user);
		await DatabaseContextFixture.SaveChangesAsync();
		
		var conversation = new CreateConversationCommand
		{
			RequesterId = user.Id,
			Name = "qwerty",
			Title = "qwery"
		};

		await MessengerModule.RequestAsync(conversation, CancellationToken.None);
	}
}