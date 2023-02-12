using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.BusinessLogic.ApiQueries.Chats;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiQueries.GetChatQueryHandlerTests;

public class GetConversationTestSuccess : IntegrationTestBase, IIntegrationTest
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
		
		await MessengerModule.RequestAsync(
			new JoinToChatCommand(
				RequesterId: alice.Value.Id,
				ChatId: conversation.Value.Id), CancellationToken.None);

		var createRoleAliceBy21ThCommand = new CreateOrUpdateRoleUserInConversationCommand(
			RequesterId: user21Th.Value.Id,
			ChatId: conversation.Value.Id,
			UserId: alice.Value.Id,
			RoleTitle: "aliceRole",
			RoleColor.Blue,
			CanBanUser: false,
			CanChangeChatData: false,
			CanAddAndRemoveUserToConversation: false,
			CanGivePermissionToUser: false);

		await MessengerModule.RequestAsync(createRoleAliceBy21ThCommand, CancellationToken.None);
		
		var getChatBy21ThQuery = new GetChatQuery(
			RequesterId: user21Th.Value.Id,
			ChatId: conversation.Value.Id);

		var getChatByAliceQuery = new GetChatQuery(
			RequesterId: alice.Value.Id,
			ChatId: conversation.Value.Id);
		
		var getChatByBobQuery =new GetChatQuery(
			RequesterId: bob.Value.Id,
			ChatId: conversation.Value.Id);
		
		var getChatBy21ThResult = await MessengerModule.RequestAsync(getChatBy21ThQuery, CancellationToken.None);
		var getChatByAliceResult = await MessengerModule.RequestAsync(getChatByAliceQuery, CancellationToken.None);
		var getChatByBobResult = await MessengerModule.RequestAsync(getChatByBobQuery, CancellationToken.None);

		getChatBy21ThResult.Value.IsOwner.Should().BeTrue();
		getChatBy21ThResult.Value.IsMember.Should().BeTrue();
		getChatBy21ThResult.Value.MembersCount.Should().Be(2);
		getChatBy21ThResult.Value.UsersWithRole[0].UserId.Should().Be(alice.Value.Id);
		getChatBy21ThResult.Value.UsersWithRole[0].ChatId.Should().Be(conversation.Value.Id);
		getChatBy21ThResult.Value.UsersWithRole[0].RoleTitle.Should().Be(createRoleAliceBy21ThCommand.RoleTitle);
		getChatBy21ThResult.Value.UsersWithRole[0].RoleColor.Should().Be(createRoleAliceBy21ThCommand.RoleColor);
		getChatBy21ThResult.Value.UsersWithRole[0].CanBanUser.Should().Be(createRoleAliceBy21ThCommand.CanBanUser);
		getChatBy21ThResult.Value.UsersWithRole[0].CanChangeChatData.Should().Be(createRoleAliceBy21ThCommand.CanChangeChatData);
		getChatBy21ThResult.Value.UsersWithRole[0].CanGivePermissionToUser.Should().Be(createRoleAliceBy21ThCommand.CanGivePermissionToUser);
		getChatBy21ThResult.Value.UsersWithRole[0].CanAddAndRemoveUserToConversation.Should().Be(createRoleAliceBy21ThCommand.CanAddAndRemoveUserToConversation);
		
		getChatByAliceResult.Value.IsOwner.Should().BeFalse();
		getChatByAliceResult.Value.IsMember.Should().BeTrue();
		
		getChatByBobResult.Value.IsOwner.Should().BeFalse();
		getChatByBobResult.Value.IsMember.Should().BeFalse();
	}
}