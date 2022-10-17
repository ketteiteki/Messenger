using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.BusinessLogic.ApiQueries.Conversations;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiQueries.GetConversationQueryHandlerTests;

public class GetConversationTestSuccess : IntegrationTestBase, IIntegrationTest
{
	[Fact]
	public async Task Test()
	{
		var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
		var alice = await MessengerModule.RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);
		var bob = await MessengerModule.RequestAsync(CommandHelper.RegistrationBobCommand(), CancellationToken.None);
		
		var createConversationCommand = new CreateConversationCommand(
			RequesterId: user21Th.Value.Id,
			Name: "qwerty",
			Title: "qwerty",
			AvatarFile: null);
		
		var conversation = await MessengerModule.RequestAsync(createConversationCommand, CancellationToken.None);
		
		await MessengerModule.RequestAsync(
			new JoinToChatCommand(
				RequesterId: alice.Value.Id,
				ChatId: conversation.Value.Id), CancellationToken.None);

		var queryFor21Th = new GetConversationQuery(
			RequesterId: user21Th.Value.Id,
			ChatId: conversation.Value.Id);

		var queryForAlice = new GetConversationQuery(
			RequesterId: alice.Value.Id,
			ChatId: conversation.Value.Id);
		
		var queryForBob =new GetConversationQuery(
			RequesterId: bob.Value.Id,
			ChatId: conversation.Value.Id);
		
		var resultFor21Th = await MessengerModule.RequestAsync(queryFor21Th, CancellationToken.None);
		var resultForAlice = await MessengerModule.RequestAsync(queryForAlice, CancellationToken.None);
		var resultForBob = await MessengerModule.RequestAsync(queryForBob, CancellationToken.None);

		resultFor21Th.Value.IsOwner.Should().BeTrue();
		resultFor21Th.Value.IsMember.Should().BeTrue();
		resultFor21Th.Value.MembersCount.Should().Be(2);
		
		resultForAlice.Value.IsOwner.Should().BeFalse();
		resultForAlice.Value.IsMember.Should().BeTrue();
		
		resultForBob.Value.IsOwner.Should().BeFalse();
		resultForBob.Value.IsMember.Should().BeFalse();
	}
}