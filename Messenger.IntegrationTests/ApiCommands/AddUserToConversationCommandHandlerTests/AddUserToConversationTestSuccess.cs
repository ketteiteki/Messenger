using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.BusinessLogic.ApiQueries.Chats;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.AddUserToConversationCommandHandlerTests;

public class AddUserToConversationTestSuccess : IntegrationTestBase, IIntegrationTest
{
	[Fact]
	public async Task Test()
	{
		var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
		var alice = await MessengerModule.RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);
		var bob = await MessengerModule.RequestAsync(CommandHelper.RegistrationBobCommand(), CancellationToken.None);

		var createConversationCommand = new CreateChatCommand(
			RequesterId: user21Th.Value.Id,
			Name: "qwerty",
			Title: "qwerty",
			Type: ChatType.Conversation,
			AvatarFile: null);
		
		var conversation = await MessengerModule.RequestAsync(createConversationCommand, CancellationToken.None);

		var addUserAliceToConversationBy21ThCommand = new AddUserToConversationCommand(
			RequesterId: user21Th.Value.Id,
			ChatId: conversation.Value.Id,
			UserId: alice.Value.Id);

		var addUserAliceToConversationBy21ThResult = 
			await MessengerModule.RequestAsync(addUserAliceToConversationBy21ThCommand, CancellationToken.None);

		var createRoleAliceInConversationCommand = new CreateOrUpdateRoleUserInConversationCommand(
			RequesterId: user21Th.Value.Id,
			UserId: alice.Value.Id,
			ChatId: conversation.Value.Id,
			RoleColor: RoleColor.Red,
			RoleTitle: "moderator",
			CanBanUser: false,
			CanChangeChatData: false,
			CanGivePermissionToUser: false,
			CanAddAndRemoveUserToConversation: true);

		await MessengerModule.RequestAsync(createRoleAliceInConversationCommand, CancellationToken.None);

		var addUserBobToConversationByAliceCommand = new AddUserToConversationCommand(
			RequesterId: alice.Value.Id,
			ChatId: conversation.Value.Id,
			UserId: bob.Value.Id);

		var addUserBobToConversationByAliceResult = 
			await MessengerModule.RequestAsync(addUserBobToConversationByAliceCommand, CancellationToken.None);

		var getConversationCommand = new GetChatQuery(
			RequesterId: bob.Value.Id,
			ChatId: conversation.Value.Id);

		var conversationForCheck = await MessengerModule.RequestAsync(getConversationCommand, CancellationToken.None);
		
		conversationForCheck.Value.IsMember.Should().BeTrue();
		conversationForCheck.Value.MembersCount.Should().Be(3);

		addUserAliceToConversationBy21ThResult.Value.Id.Should().Be(alice.Value.Id);
		addUserBobToConversationByAliceResult.Value.Id.Should().Be(bob.Value.Id);
	}
}