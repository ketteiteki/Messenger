using FluentAssertions;
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
		var user21th = await MessengerModule.RequestAsync(CommandHelper.Registration21thCommand(), CancellationToken.None);
		var alice = await MessengerModule.RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);
		var bob = await MessengerModule.RequestAsync(CommandHelper.RegistrationBobCommand(), CancellationToken.None);
		
		var createConversationCommand = new CreateConversationCommand(
			RequestorId: user21th.Value.Id,
			Name: "qwerty",
			Title: "qwerty",
			AvatarFile: null);
		
		var conversation = await MessengerModule.RequestAsync(createConversationCommand, CancellationToken.None);
		
		await MessengerModule.RequestAsync(
			new JoinToConversationCommand(
				RequestorId: alice.Value.Id,
				ChatId: conversation.Value.Id), CancellationToken.None);

		var queryFor21th = new GetConversationQuery(
			RequestorId: user21th.Value.Id,
			ChatId: conversation.Value.Id);

		var queryForAlice = new GetConversationQuery(
			RequestorId: alice.Value.Id,
			ChatId: conversation.Value.Id);
		
		var queryForBob =new GetConversationQuery(
			RequestorId: bob.Value.Id,
			ChatId: conversation.Value.Id);
		
		var resultFor21th = await MessengerModule.RequestAsync(queryFor21th, CancellationToken.None);
		var resultForAlice = await MessengerModule.RequestAsync(queryForAlice, CancellationToken.None);
		var resultForBob = await MessengerModule.RequestAsync(queryForBob, CancellationToken.None);

		resultFor21th.Value.IsOwner.Should().BeTrue();
		resultFor21th.Value.IsMember.Should().BeTrue();
		resultFor21th.Value.MembersCount.Should().Be(2);
		
		resultForAlice.Value.IsOwner.Should().BeFalse();
		resultForAlice.Value.IsMember.Should().BeTrue();
		
		resultForBob.Value.IsOwner.Should().BeFalse();
		resultForBob.Value.IsMember.Should().BeFalse();
	}
}