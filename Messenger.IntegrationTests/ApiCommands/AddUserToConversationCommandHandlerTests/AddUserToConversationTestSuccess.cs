using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.BusinessLogic.ApiQueries.Chats;
using Messenger.Domain.Enums;
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
			user21Th.Value.Id,
			Name: "qwerty",
			Title: "qwerty",
			ChatType.Conversation,
			AvatarFile: null);
		
		var createConversationResult = await MessengerModule.RequestAsync(createConversationCommand, CancellationToken.None);

		var addUserAliceToConversationBy21ThCommand = new AddUserToConversationCommand(
			user21Th.Value.Id,
			createConversationResult.Value.Id,
			alice.Value.Id);

		var addUserAliceToConversationBy21ThResult = 
			await MessengerModule.RequestAsync(addUserAliceToConversationBy21ThCommand, CancellationToken.None);

		var createRoleAliceInConversationCommand = new CreateOrUpdateRoleUserInConversationCommand(
			user21Th.Value.Id,
			createConversationResult.Value.Id,
			alice.Value.Id,
			RoleTitle: "moderator",
			RoleColor.Red,
			CanBanUser: false,
			CanChangeChatData: false,
			CanAddAndRemoveUserToConversation: true,
			CanGivePermissionToUser: false);

		await MessengerModule.RequestAsync(createRoleAliceInConversationCommand, CancellationToken.None);

		var addUserBobToConversationByAliceCommand = new AddUserToConversationCommand(
			alice.Value.Id,
			createConversationResult.Value.Id,
			bob.Value.Id);

		var addUserBobToConversationByAliceResult = 
			await MessengerModule.RequestAsync(addUserBobToConversationByAliceCommand, CancellationToken.None);

		var getConversationCommand = new GetChatQuery(
			bob.Value.Id,
			createConversationResult.Value.Id);

		var getConversationResult = await MessengerModule.RequestAsync(getConversationCommand, CancellationToken.None);
		
		getConversationResult.Value.IsMember.Should().BeTrue();
		getConversationResult.Value.MembersCount.Should().Be(3);

		addUserAliceToConversationBy21ThResult.Value.Id.Should().Be(alice.Value.Id);
		addUserBobToConversationByAliceResult.Value.Id.Should().Be(bob.Value.Id);
	}
}