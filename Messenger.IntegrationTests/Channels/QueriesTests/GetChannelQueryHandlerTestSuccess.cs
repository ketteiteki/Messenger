using FluentAssertions;
using Messenger.BusinessLogic.Channels.Queries;
using Messenger.Domain.Entities;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.Channels.QueriesTests;

public class GetChannelQueryHandlerTestSuccess : IntegrationTestBase, IIntegrationTest
{
	[Fact]
	public async Task Test()
	{
		var user21th = EntityHelper.CreateUser21th();
		var bob = EntityHelper.CreateUserBob();
		var alice = EntityHelper.CreateUserAlice();
		
		DatabaseContextFixture.Users.AddRange(user21th, bob, alice);
		await DatabaseContextFixture.SaveChangesAsync();

		var chatEntity = EntityHelper.CreateChannel(user21th.Id, "convers", "21ths den");

		DatabaseContextFixture.Chats.AddRange(chatEntity);
		await DatabaseContextFixture.SaveChangesAsync();
		
		chatEntity.ChatUsers.Add(new ChatUser {UserId = user21th.Id, ChatId = chatEntity.Id});
		chatEntity.ChatUsers.Add(new ChatUser {UserId = bob.Id, ChatId = chatEntity.Id, CanSendMedia = false});
		
		DatabaseContextFixture.Chats.Update(chatEntity);
		await DatabaseContextFixture.SaveChangesAsync();
		
		var queryForRequester = new GetChannelQuery { RequesterId = user21th.Id, ChannelId = chatEntity.Id };
		var queryForAlice = new GetChannelQuery { RequesterId = alice.Id, ChannelId = chatEntity.Id };
		var queryForBob = new GetChannelQuery { RequesterId = bob.Id, ChannelId = chatEntity.Id };

		var chatForRequester = await MessengerModule.RequestAsync(queryForRequester, CancellationToken.None);
		var chatForAlice = await MessengerModule.RequestAsync(queryForAlice, CancellationToken.None);
		var chatForBob = await MessengerModule.RequestAsync(queryForBob, CancellationToken.None);

		chatForRequester.IsMember.Should().Be(true);
		chatForRequester.IsOwner.Should().Be(true);
		chatForRequester.MembersCount.Should().Be(2);
		chatForRequester.CanSendMedia.Should().Be(true);
		
		chatForAlice.IsMember.Should().Be(false);
		chatForAlice.IsOwner.Should().Be(false);
		chatForAlice.MembersCount.Should().Be(2);
		chatForAlice.CanSendMedia.Should().Be(false);
		
		chatForBob.IsMember.Should().Be(true);
		chatForBob.IsOwner.Should().Be(false);
		chatForBob.MembersCount.Should().Be(2);
		chatForBob.CanSendMedia.Should().Be(false);
	}
}