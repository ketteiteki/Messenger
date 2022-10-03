using Messenger.BusinessLogic.Channels.Queries;
using Messenger.Domain.Entities;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.Channels.QueriesTests;

public class GetChannelQueryHandlerTestAccess : IntegrationTestBase, IIntegrationTest
{
	[Fact]
	public async Task Test()
	{
		var requester = EntityHelper.CreateUser21th();

		var user = EntityHelper.CreateUserKhachatur();
		
		DatabaseContextFixture.Users.AddRange(requester, user);
		await DatabaseContextFixture.SaveChangesAsync();

		var chatEntity = EntityHelper.CreateChannel(requester.Id, "convers", "21ths den");

		DatabaseContextFixture.Chats.AddRange(chatEntity);
		await DatabaseContextFixture.SaveChangesAsync();
		
		chatEntity.ChatUsers.Add(new ChatUser {UserId = requester.Id, ChatId = chatEntity.Id});
		chatEntity.ChatUsers.Add(new ChatUser {UserId = user.Id, ChatId = chatEntity.Id, CanSendMedia = false});
		
		DatabaseContextFixture.Chats.Update(chatEntity);
		await DatabaseContextFixture.SaveChangesAsync();
		
		var query = new GetChannelQuery { RequesterId = user.Id, ChannelId = chatEntity.Id };

		var chat = await MessengerModule.RequestAsync(query, CancellationToken.None);
	}
}