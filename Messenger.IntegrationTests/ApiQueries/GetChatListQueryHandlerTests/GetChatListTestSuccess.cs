using FluentAssertions;
using Messenger.BusinessLogic.ApiQueries.Chats;
using Messenger.Domain.Entities;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiQueries.GetChatListQueryHandlerTests;

public class GetChatListTestSuccess : IntegrationTestBase, IIntegrationTest
{
	[Fact]
	public async Task Test()
	{
		var user21th = EntityHelper.CreateUser21th();
		var alice = EntityHelper.CreateUserAlice();

		DatabaseContextFixture.Users.AddRange(user21th, alice);
		await DatabaseContextFixture.SaveChangesAsync();
		
		var conv1 = EntityHelper.CreateChannel(user21th.Id, "conv1", "21th conv1");
		var conv2 = EntityHelper.CreateChannel(user21th.Id, "conv2", "21th conv2");
		var conv3 = EntityHelper.CreateChannel(user21th.Id, "conv3", "21th conv3");

		var dialog = EntityHelper.CreateDialog();
		
		DatabaseContextFixture.Chats.AddRange(conv1, conv2, conv3, dialog);
		await DatabaseContextFixture.SaveChangesAsync();
		
		DatabaseContextFixture.ChatUsers.AddRange(
		new ChatUser {UserId = user21th.Id, ChatId = conv1.Id},
		new ChatUser {UserId = user21th.Id, ChatId = conv2.Id},
		new ChatUser {UserId = user21th.Id, ChatId = conv3.Id},
		new ChatUser {UserId = alice.Id, ChatId = conv3.Id},
		new ChatUser {UserId = alice.Id, ChatId = dialog.Id},
		new ChatUser {UserId = user21th.Id, ChatId = dialog.Id});

		await DatabaseContextFixture.SaveChangesAsync();
		
		var queryFor21th = new GetChatListQuery(RequestorId: user21th.Id);
		var queryForAlice = new GetChatListQuery(RequestorId: alice.Id);

		var chatListFor21th = await MessengerModule.RequestAsync(queryFor21th, CancellationToken.None);
		var chatListForAlice = await MessengerModule.RequestAsync(queryForAlice, CancellationToken.None);

		foreach (var chat in chatListFor21th)
		{
			if (chat.Type == ChatType.Dialog)
			{
				chat.IsMember.Should().Be(true);
				chat.IsOwner.Should().Be(false);
				chat.Members.Count.Should().Be(2);
				
				continue;
			}
			
			chat.IsMember.Should().Be(true);
			chat.IsOwner.Should().Be(true);
		}
		
		foreach (var chat in chatListForAlice)
		{
			chat.IsMember.Should().Be(true);
			chat.IsOwner.Should().Be(false);
		}
	}
}