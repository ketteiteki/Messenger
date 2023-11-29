using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Enums;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.UnbanUserInConversationCommandHandlerTests;

public class UnbanUserInConversationTestThrowForbidden : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
		var user21Th = await RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
		var alice = await RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);
		var bob = await RequestAsync(CommandHelper.RegistrationBobCommand(), CancellationToken.None);

		var createConversationCommand = new CreateChatCommand(
			user21Th.Value.Id,
			Name: "qwerty",
			Title: "qwerty",
			ChatType.Conversation,
			AvatarFile: null);

		var createConversationResult = await RequestAsync(createConversationCommand, CancellationToken.None);

		var aliceJoinConversationCommand = new JoinToChatCommand(alice.Value.Id, createConversationResult.Value.Id);
		var bobJoinConversationCommand = new JoinToChatCommand(bob.Value.Id, createConversationResult.Value.Id);
		
		await RequestAsync(aliceJoinConversationCommand, CancellationToken.None);
		await RequestAsync(bobJoinConversationCommand, CancellationToken.None);
		
		var banAliceCommand = new BanUserInConversationCommand(
			user21Th.Value.Id,
			createConversationResult.Value.Id,
			alice.Value.Id,
			BanMinutes: 15);
		
		await RequestAsync(banAliceCommand, CancellationToken.None);
		
		var unbanAliceInConversationByBobCommand = new UnbanUserInConversationCommand(
			bob.Value.Id,
			createConversationResult.Value.Id,
			alice.Value.Id);
		
		var unbanUserAliceInConversationByBobResult = 
			await RequestAsync(unbanAliceInConversationByBobCommand, CancellationToken.None);

		unbanUserAliceInConversationByBobResult.Error.Should().BeOfType<ForbiddenError>();
    }
}