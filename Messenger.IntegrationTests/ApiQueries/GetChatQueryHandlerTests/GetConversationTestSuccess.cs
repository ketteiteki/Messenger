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
			user21Th.Value.Id,
			Name: "qwerty",
			Title: "qwerty",
			ChatType.Conversation,
			AvatarFile: null);

		var createConversationResult = await MessengerModule.RequestAsync(createConversationCommand, CancellationToken.None);

		var aliceJoinConversationCommand = new JoinToChatCommand(alice.Value.Id, createConversationResult.Value.Id);
		
		await MessengerModule.RequestAsync(aliceJoinConversationCommand, CancellationToken.None);

		var createAliceRoleBy21ThCommand = new CreateOrUpdateRoleUserInConversationCommand(
			user21Th.Value.Id,
			createConversationResult.Value.Id,
			alice.Value.Id,
			RoleTitle: "aliceRole",
			RoleColor.Blue,
			CanBanUser: false,
			CanChangeChatData: false,
			CanAddAndRemoveUserToConversation: false,
			CanGivePermissionToUser: false);

		await MessengerModule.RequestAsync(createAliceRoleBy21ThCommand, CancellationToken.None);
		
		var getChatBy21ThQuery = new GetChatQuery(user21Th.Value.Id, createConversationResult.Value.Id);
		var getChatByAliceQuery = new GetChatQuery(alice.Value.Id, createConversationResult.Value.Id);
		var getChatByBobQuery =new GetChatQuery(bob.Value.Id, createConversationResult.Value.Id);
		
		var getChatBy21ThResult = await MessengerModule.RequestAsync(getChatBy21ThQuery, CancellationToken.None);
		var getChatByAliceResult = await MessengerModule.RequestAsync(getChatByAliceQuery, CancellationToken.None);
		var getChatByBobResult = await MessengerModule.RequestAsync(getChatByBobQuery, CancellationToken.None);

		getChatBy21ThResult.Value.IsOwner.Should().BeTrue();
		getChatBy21ThResult.Value.IsMember.Should().BeTrue();
		getChatBy21ThResult.Value.MembersCount.Should().Be(2);
		
		getChatBy21ThResult.Value.UsersWithRole[0].UserId.Should().Be(alice.Value.Id);
		getChatBy21ThResult.Value.UsersWithRole[0].ChatId.Should().Be(createConversationResult.Value.Id);
		getChatBy21ThResult.Value.UsersWithRole[0].RoleTitle.Should().Be(createAliceRoleBy21ThCommand.RoleTitle);
		getChatBy21ThResult.Value.UsersWithRole[0].RoleColor.Should().Be(createAliceRoleBy21ThCommand.RoleColor);
		getChatBy21ThResult.Value.UsersWithRole[0].CanBanUser.Should().Be(createAliceRoleBy21ThCommand.CanBanUser);
		
		getChatBy21ThResult.Value.UsersWithRole[0].CanChangeChatData.Should()
			.Be(createAliceRoleBy21ThCommand.CanChangeChatData);
		getChatBy21ThResult.Value.UsersWithRole[0].CanGivePermissionToUser.Should()
			.Be(createAliceRoleBy21ThCommand.CanGivePermissionToUser);
		getChatBy21ThResult.Value.UsersWithRole[0].CanAddAndRemoveUserToConversation.Should()
			.Be(createAliceRoleBy21ThCommand.CanAddAndRemoveUserToConversation);

		getChatByAliceResult.Value.IsOwner.Should().BeFalse();
		getChatByAliceResult.Value.IsMember.Should().BeTrue();
		
		getChatByBobResult.Value.IsOwner.Should().BeFalse();
		getChatByBobResult.Value.IsMember.Should().BeFalse();
	}
}