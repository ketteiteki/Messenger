using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.BusinessLogic.ApiCommands.Dialogs;
using Messenger.BusinessLogic.ApiCommands.Messages;
using Messenger.BusinessLogic.ApiQueries.Chats;
using Messenger.Domain.Constants;
using Messenger.Domain.Enums;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiQueries.GetChatListQueryHandlerTests;

public class GetChatListTestSuccess : IntegrationTestBase, IIntegrationTest
{
	[Fact]
	public async Task Test()
	{
		var user21Th = await RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
		var alice = await RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);

		var firstCreateConversationCommand = new CreateChatCommand(
			user21Th.Value.Id,
			Name: "conv1",
			Title: "conv1",
			ChatType.Conversation,
			AvatarFile: null);
		
		var secondCreateConversationCommand = new CreateChatCommand(
			user21Th.Value.Id,
			Name: "conv2",
			Title: "conv2",
			ChatType.Conversation,
			AvatarFile: null);
		
		var thirdCreateConversationCommand = new CreateChatCommand(
			user21Th.Value.Id,
			Name: "conv3",
			Title: "conv3",
			ChatType.Conversation,
			AvatarFile: null);

		var createDialogCommand = new CreateDialogCommand(user21Th.Value.Id, alice.Value.Id);

		var firstCreateConversationResult = 
			await RequestAsync(firstCreateConversationCommand, CancellationToken.None);
		
		await RequestAsync(secondCreateConversationCommand, CancellationToken.None);
		
		var thirdCreateConversationResult = await RequestAsync(thirdCreateConversationCommand, CancellationToken.None);
		
		await RequestAsync(createDialogCommand, CancellationToken.None);

		var createMessageBy21ThCommand = new CreateMessageCommand(
			user21Th.Value.Id,
			Text: "Hello",
			ReplyToId: null,
			firstCreateConversationResult.Value.Id,
			Files: null);
		
		var createMessageBy21ThResult = await RequestAsync(createMessageBy21ThCommand, CancellationToken.None);

		var aliceJoinThirdConversation = new JoinToChatCommand(alice.Value.Id, thirdCreateConversationResult.Value.Id);
		
		await RequestAsync(aliceJoinThirdConversation, CancellationToken.None);

		var createAliceRoleInThirdConversationBy21ThCommand = new CreateOrUpdateRoleUserInConversationCommand(
			user21Th.Value.Id,
			thirdCreateConversationResult.Value.Id,
			alice.Value.Id,
			RoleTitle: "qwerty",
			RoleColor.Cyan,
			CanBanUser: true,
			CanChangeChatData: false,
			CanAddAndRemoveUserToConversation: true,
			CanGivePermissionToUser: false);
		
		await RequestAsync(createAliceRoleInThirdConversationBy21ThCommand, CancellationToken.None);

		var getChatListBy21ThQuery = new GetChatListQuery(user21Th.Value.Id);
		var getChatListByAliceQuery = new GetChatListQuery(alice.Value.Id);
		
		var getChatListBy21ThResult = await RequestAsync(getChatListBy21ThQuery, CancellationToken.None);
		var getChatListByAliceResult = await RequestAsync(getChatListByAliceQuery, CancellationToken.None);

		foreach (var chat in getChatListBy21ThResult.Value)
		{
			if (chat.Id == firstCreateConversationResult.Value.Id)
			{
				chat.LastMessageId.Should().Be(createMessageBy21ThResult.Value.Id);
				chat.LastMessageText.Should().Be(createMessageBy21ThResult.Value.Text);
				chat.LastMessageAuthorDisplayName.Should().Be(createMessageBy21ThResult.Value.OwnerDisplayName);
				chat.LastMessageDateOfCreate.Should().NotBeNull();
			}
			
			if (chat.Type == ChatType.Dialog)
			{
				chat.IsMember.Should().Be(true);
				chat.IsOwner.Should().Be(false);
				chat.Members.Count.Should().Be(2);
				
				continue;
			}
			
			chat.IsMember.Should().Be(true);

			if (chat.Id == SeedDataConstants.DotnetChatId || chat.Id == SeedDataConstants.DotnetFloodChatId)
			{
				chat.IsOwner.Should().Be(false);
				continue;
			}
			
			chat.IsOwner.Should().Be(true);
		}
		
		foreach (var chat in getChatListByAliceResult.Value)
		{
			chat.IsMember.Should().Be(true);
			chat.IsOwner.Should().Be(false);
		}
	}
}