using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.BusinessLogic.ApiQueries.Chats;
using Messenger.Domain.Enums;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiQueries.GetChatQueryHandlerTests;

public class GetChannelTestSuccess : IntegrationTestBase, IIntegrationTest
{
	[Fact]
	public async Task Test()
	{
		var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
		var bob = await MessengerModule.RequestAsync(CommandHelper.RegistrationBobCommand(), CancellationToken.None);
		var alice = await MessengerModule.RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);

		var createChannelCommand = new CreateChatCommand(
			RequesterId: user21Th.Value.Id,
			Name: "qwerty",
			Title: "qwerty",
			Type: ChatType.Channel,
			AvatarFile: null);

		var createChannelResult = await MessengerModule.RequestAsync(createChannelCommand, CancellationToken.None);

		var bobJoinChannelCommand = new JoinToChatCommand(bob.Value.Id, createChannelResult.Value.Id);
		
		await MessengerModule.RequestAsync(bobJoinChannelCommand, CancellationToken.None);

		var createBobPermissionsBy21ThCommand = new CreatePermissionsUserInConversationCommand(
			user21Th.Value.Id,
			createChannelResult.Value.Id,
			bob.Value.Id,
			CanSendMedia: false,
			MuteMinutes: null);

		await MessengerModule.RequestAsync(createBobPermissionsBy21ThCommand, CancellationToken.None);

		var getChannelBy21ThQuery = new GetChatQuery(user21Th.Value.Id, createChannelResult.Value.Id);
		var getChannelByAliceQuery = new GetChatQuery(alice.Value.Id, createChannelResult.Value.Id);
		var getChannelByBobQuery = new GetChatQuery(bob.Value.Id, createChannelResult.Value.Id);

		var getChannelBy21ThResult = await MessengerModule.RequestAsync(getChannelBy21ThQuery, CancellationToken.None);
		var getChannelByAliceResult = await MessengerModule.RequestAsync(getChannelByAliceQuery, CancellationToken.None);
		var getChannelByBobResult = await MessengerModule.RequestAsync(getChannelByBobQuery, CancellationToken.None);

		getChannelBy21ThResult.Value.IsMember.Should().Be(true);
		getChannelBy21ThResult.Value.IsOwner.Should().Be(true);
		getChannelBy21ThResult.Value.MembersCount.Should().Be(2);
		getChannelBy21ThResult.Value.CanSendMedia.Should().Be(true);
		
		getChannelByAliceResult.Value.IsMember.Should().Be(false);
		getChannelByAliceResult.Value.IsOwner.Should().Be(false);
		getChannelByAliceResult.Value.MembersCount.Should().Be(2);
		getChannelByAliceResult.Value.CanSendMedia.Should().Be(false);
		
		getChannelByBobResult.Value.IsMember.Should().Be(true);
		getChannelByBobResult.Value.IsOwner.Should().Be(false);
		getChannelByBobResult.Value.MembersCount.Should().Be(2);
		getChannelByBobResult.Value.CanSendMedia.Should().Be(false);
	}
}