using FluentAssertions;
using Messenger.BusinessLogic.Chats.Queries;
using Messenger.Domain.Entities;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.Chats.QueriesTests;

public class GetChatQueryHandlerTestSuccess : IntegrationTestBase, IIntegrationTest
{
	[Fact]
	public async Task Test()
	{
		var user21th = EntityHelper.CreateUser21th();
		var alice = EntityHelper.CreateUserAlice();

		DatabaseContextFixture.Users.AddRange(user21th, alice);
		await DatabaseContextFixture.SaveChangesAsync();
		
		var conversation = EntityHelper.CreateChannel(user21th.Id, "conv1", "21th conv1");

		var dialog = EntityHelper.CreateDialog();
		
		DatabaseContextFixture.Chats.AddRange(conversation, dialog);
		await DatabaseContextFixture.SaveChangesAsync();
		
		DatabaseContextFixture.ChatUsers.AddRange(
			new ChatUser {UserId = user21th.Id, ChatId = conversation.Id},
			new ChatUser {UserId = alice.Id, ChatId = dialog.Id},
			new ChatUser {UserId = user21th.Id, ChatId = dialog.Id});

		await DatabaseContextFixture.SaveChangesAsync();
		
		var queryFor21th = new GetChatQuery { RequesterId = user21th.Id, ChatId = conversation.Id};
		var queryForAlice = new GetChatQuery { RequesterId = alice.Id, ChatId = conversation.Id};
		var queryForAliceDialog = new GetChatQuery { RequesterId = alice.Id, ChatId = dialog.Id};

		var chatFor21th = await MessengerModule.RequestAsync(queryFor21th, CancellationToken.None);
		var chatForAlice = await MessengerModule.RequestAsync(queryForAlice, CancellationToken.None);
		var chatForAliceDialog = await MessengerModule.RequestAsync(queryForAliceDialog, CancellationToken.None);
		
		if (chatFor21th.Type == ChatType.Dialog)
		{
			chatFor21th.IsMember.Should().Be(true);
			chatFor21th.IsOwner.Should().Be(false);
			chatFor21th.Members.Count.Should().Be(2);
		}
		else
		{
			chatFor21th.IsMember.Should().Be(true);
			chatFor21th.IsOwner.Should().Be(true);
		}
			
		chatForAlice.IsMember.Should().Be(true);
		chatForAlice.IsOwner.Should().Be(false);

		chatForAliceDialog.IsOwner.Should().Be(false);
		chatForAliceDialog.IsOwner.Should().Be(false);
		chatForAliceDialog.Members.Count.Should().Be(2);
	}
}