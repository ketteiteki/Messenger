using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.BusinessLogic.ApiCommands.Dialogs;
using Messenger.BusinessLogic.ApiQueries.Chats;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiQueries.GetChatQueryHandlerTests;

public class GetChatTestSuccess : IntegrationTestBase, IIntegrationTest
{
	[Fact]
	public async Task Test()
	{
		var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
		var alice = await MessengerModule.RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);

		var createConversationCommand = new CreateConversationCommand(
			RequesterId: user21Th.Value.Id,
			Name: "qwerty",
			Title: "qwerty",
			AvatarFile: null);

		var createDialogCommand = new CreateDialogCommand(
			RequesterId: user21Th.Value.Id,
			UserId: alice.Value.Id);

		var conversation = await MessengerModule.RequestAsync(createConversationCommand, CancellationToken.None);
		var dialog = await MessengerModule.RequestAsync(createDialogCommand, CancellationToken.None);

		await MessengerModule.RequestAsync(
			new JoinToChatCommand(
				RequesterId: alice.Value.Id,
				ChatId: conversation.Value.Id), CancellationToken.None);
		
		var conversationFor21ThQuery = new GetChatQuery(
			RequesterId: user21Th.Value.Id,
			ChatId: conversation.Value.Id);
		var conversationForAliceQuery = new GetChatQuery(
			RequesterId: alice.Value.Id, 
			ChatId: conversation.Value.Id);
		var dialogForAliceQuery = new GetChatQuery(
			RequesterId: alice.Value.Id, 
			ChatId: dialog.Value.Id);

		var conversationFor21Th = await MessengerModule.RequestAsync(conversationFor21ThQuery, CancellationToken.None);
		var conversationForAlice = await MessengerModule.RequestAsync(conversationForAliceQuery, CancellationToken.None);
		var dialogForAlice = await MessengerModule.RequestAsync(dialogForAliceQuery, CancellationToken.None);
		
		if (conversationFor21Th.Value.Type == ChatType.Dialog)
		{
			conversationFor21Th.Value.IsMember.Should().Be(true);
			conversationFor21Th.Value.IsOwner.Should().Be(false);
			conversationFor21Th.Value.Members.Count.Should().Be(2);
		}
		else
		{
			conversationFor21Th.Value.IsMember.Should().Be(true);
			conversationFor21Th.Value.IsOwner.Should().Be(true);
		}
			
		conversationForAlice.Value.IsMember.Should().Be(true);
		conversationForAlice.Value.IsOwner.Should().Be(false);

		dialogForAlice.Value.IsOwner.Should().Be(false);
		dialogForAlice.Value.IsOwner.Should().Be(false);
		dialogForAlice.Value.Members.Count.Should().Be(2);
	}
}