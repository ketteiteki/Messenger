using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;

namespace Messenger.IntegrationTests.Chats.QueriesTests;

public class GetChatListQueryHandlerAccess : IntegrationTestBase, IIntegrationTest
{
	public async Task Test()
	{
		var user21th = EntityHelper.CreateUser21th();
		var alice = EntityHelper.CreateUserAlice();

		DatabaseContextFixture.Users.AddRange(user21th, alice);
		await DatabaseContextFixture.SaveChangesAsync();
		
		var conv1 = EntityHelper.CreateChannel(user21th.Id, "conv1", "21th conv1");
		var conv2 = EntityHelper.CreateChannel(user21th.Id, "conv2", "21th conv2");
		var conv3 = EntityHelper.CreateChannel(user21th.Id, "conv3", "21th conv3");
		
		
	}
}