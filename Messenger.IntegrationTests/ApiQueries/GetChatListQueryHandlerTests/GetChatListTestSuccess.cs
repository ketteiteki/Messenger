using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.BusinessLogic.ApiCommands.Dialogs;
using Messenger.BusinessLogic.ApiQueries.Chats;
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
		var user21th = await MessengerModule.RequestAsync(CommandHelper.Registration21thCommand(), CancellationToken.None);
		var alice = await MessengerModule.RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);

		var createConversationCommand1 = new CreateConversationCommand(
			RequestorId: user21th.Value.Id,
			Name: "conv1",
			Title: "conv1",
			AvatarFile: null);
		
		var createConversationCommand2 = new CreateConversationCommand(
			RequestorId: user21th.Value.Id,
			Name: "conv2",
			Title: "conv2",
			AvatarFile: null);
		
		var createConversationCommand3 = new CreateConversationCommand(
			RequestorId: user21th.Value.Id,
			Name: "conv3",
			Title: "conv3",
			AvatarFile: null);

		var createDialogCommand = new CreateDialogCommand(
			RequestorId: user21th.Value.Id,
			UserId: alice.Value.Id);

		await MessengerModule.RequestAsync(createConversationCommand1, CancellationToken.None);
		await MessengerModule.RequestAsync(createConversationCommand2, CancellationToken.None);
		var conversation3 = await MessengerModule.RequestAsync(createConversationCommand3, CancellationToken.None);
		await MessengerModule.RequestAsync(createDialogCommand, CancellationToken.None);

		await MessengerModule.RequestAsync(
			new JoinToConversationCommand(
				RequestorId: alice.Value.Id,
				ChatId: conversation3.Value.Id), CancellationToken.None);

		var queryFor21th = new GetChatListQuery(RequestorId: user21th.Value.Id);
		var queryForAlice = new GetChatListQuery(RequestorId: alice.Value.Id);

		var chatListFor21th = await MessengerModule.RequestAsync(queryFor21th, CancellationToken.None);
		var chatListForAlice = await MessengerModule.RequestAsync(queryForAlice, CancellationToken.None);

		foreach (var chat in chatListFor21th.Value)
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
		
		foreach (var chat in chatListForAlice.Value)
		{
			chat.IsMember.Should().Be(true);
			chat.IsOwner.Should().Be(false);
		}
	}
}