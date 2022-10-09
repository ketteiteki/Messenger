using FluentAssertions;
using Messenger.BusinessLogic.ApiQueries.Conversations;
using Messenger.Domain.Entities;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiQueries.GetConversationQueryHandlerTests;

public class GetConversationTestSuccess : IntegrationTestBase, IIntegrationTest
{
	[Fact]
	public async Task Test()
	{
		var user21th = EntityHelper.CreateUser21th();
		var alice = EntityHelper.CreateUserAlice();
		var bob = EntityHelper.CreateUserBob();
		
		DatabaseContextFixture.Users.AddRange(user21th, alice, bob);
		await DatabaseContextFixture.SaveChangesAsync();

		var conversation = EntityHelper.CreateConversation(user21th.Id, "qwerty", "qwerty");
		
		DatabaseContextFixture.Chats.Add(conversation);
		await DatabaseContextFixture.SaveChangesAsync();
		
		DatabaseContextFixture.ChatUsers.AddRange(
			new ChatUser {UserId = user21th.Id, ChatId = conversation.Id},
			new ChatUser {UserId = alice.Id, ChatId = conversation.Id});
		await DatabaseContextFixture.SaveChangesAsync();

		var queryFor21th = new GetConversationQuery(
			RequestorId: user21th.Id,
			ChatId: conversation.Id);

		var queryForAlice = new GetConversationQuery(
			RequestorId: alice.Id,
			ChatId: conversation.Id);
		
		var queryForBob =new GetConversationQuery(
			RequestorId: bob.Id,
			ChatId: conversation.Id);
		
		var resultFor21th = await MessengerModule.RequestAsync(queryFor21th, CancellationToken.None);
		var resultForAlice = await MessengerModule.RequestAsync(queryForAlice, CancellationToken.None);
		var resultForBob = await MessengerModule.RequestAsync(queryForBob, CancellationToken.None);

		resultFor21th.IsOwner.Should().BeTrue();
		resultFor21th.IsMember.Should().BeTrue();
		resultFor21th.MembersCount.Should().Be(2);
		
		resultForAlice.IsOwner.Should().BeFalse();
		resultForAlice.IsMember.Should().BeTrue();
		
		resultForBob.IsOwner.Should().BeFalse();
		resultForBob.IsMember.Should().BeFalse();
	}
}