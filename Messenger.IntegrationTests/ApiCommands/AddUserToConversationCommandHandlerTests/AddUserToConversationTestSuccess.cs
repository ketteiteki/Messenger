using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.BusinessLogic.ApiQueries.Conversations;
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

		var conversation = await MessengerModule.RequestAsync(
			CommandHelper.CreateConversationCommand(user21Th.Value.Id, "qwerty", "qwerty", null), CancellationToken.None);

		var addUserToConversationBy21ThCommand = new AddUserToConversationCommand(
			RequesterId: user21Th.Value.Id,
			ChatId: conversation.Value.Id,
			UserId: alice.Value.Id);

		await MessengerModule.RequestAsync(addUserToConversationBy21ThCommand, CancellationToken.None);

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

		var addUserToConversationByAliceCommand = new AddUserToConversationCommand(
			RequesterId: alice.Value.Id,
			ChatId: conversation.Value.Id,
			UserId: bob.Value.Id);

		await MessengerModule.RequestAsync(addUserToConversationByAliceCommand, CancellationToken.None);

		var getConversationCommand = new GetConversationQuery(
			RequesterId: bob.Value.Id,
			ChatId: conversation.Value.Id);

		var conversationForCheck = await MessengerModule.RequestAsync(getConversationCommand, CancellationToken.None);
		
		conversationForCheck.Value.IsMember.Should().BeTrue();
		conversationForCheck.Value.MembersCount.Should().Be(3);
	}
}