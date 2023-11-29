using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.Domain.Enums;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.UnbanUserInConversationCommandHandlerTests;

public class UnbanUserInConversationTestSuccess : IntegrationTestBase, IIntegrationTest
{
	[Fact]
	public async Task Test()
	{
		var user21Th = await RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
		var alice = await RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);
		var bob = await RequestAsync(CommandHelper.RegistrationBobCommand(), CancellationToken.None);
		var alex = await RequestAsync(CommandHelper.RegistrationAlexCommand(), CancellationToken.None);

		var createConversationCommand = new CreateChatCommand(
			user21Th.Value.Id,
			Name: "qwerty",
			Title: "qwerty",
			ChatType.Conversation,
			AvatarFile: null);

		var createConversationResult = await RequestAsync(createConversationCommand, CancellationToken.None);

		var aliceJoinConversationCommand = new JoinToChatCommand(alice.Value.Id, createConversationResult.Value.Id);
		var bobJoinConversationCommand = new JoinToChatCommand(bob.Value.Id, createConversationResult.Value.Id);
		var alexJoinConversationCommand = new JoinToChatCommand(alex.Value.Id, createConversationResult.Value.Id);
		
		await RequestAsync(aliceJoinConversationCommand, CancellationToken.None);
		await RequestAsync(bobJoinConversationCommand, CancellationToken.None);
		await RequestAsync(alexJoinConversationCommand, CancellationToken.None);
		
		var banAliceCommand = new BanUserInConversationCommand(
			user21Th.Value.Id,
			createConversationResult.Value.Id,
			alice.Value.Id,
			BanMinutes: 15);
		
		var banAlexCommand = new BanUserInConversationCommand(
			user21Th.Value.Id,
			createConversationResult.Value.Id,
			alex.Value.Id,
			BanMinutes: 15);

		await RequestAsync(banAliceCommand, CancellationToken.None);
		await RequestAsync(banAlexCommand, CancellationToken.None);
		
		var createRoleBobInConversationCommand = new CreateOrUpdateRoleUserInConversationCommand(
			user21Th.Value.Id,
			createConversationResult.Value.Id,
			bob.Value.Id,
			RoleTitle: "moderator",
			RoleColor.Cyan,
			CanBanUser: true,
			CanChangeChatData: false,
			CanAddAndRemoveUserToConversation: true,
			CanGivePermissionToUser:false);

		await RequestAsync(createRoleBobInConversationCommand, CancellationToken.None);

		var unbanAliceInConversationBy21ThCommand = new UnbanUserInConversationCommand(
			user21Th.Value.Id,
			createConversationResult.Value.Id,
			alice.Value.Id);
		
		var unbanAlexInConversationByBobCommand = new UnbanUserInConversationCommand(
			bob.Value.Id,
			createConversationResult.Value.Id,
			alex.Value.Id);

		var unbanAliceInConversationBy21ThResult = 
			await RequestAsync(unbanAliceInConversationBy21ThCommand, CancellationToken.None);
		
		var unbanAlexInConversationByBobResult = 
			await RequestAsync(unbanAlexInConversationByBobCommand, CancellationToken.None);

		unbanAliceInConversationBy21ThResult.IsSuccess.Should().BeTrue();
		unbanAlexInConversationByBobResult.IsSuccess.Should().BeTrue();
	}
}