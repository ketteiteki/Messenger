using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.UnbanUserInConversationCommandHandlerTests;

public class UnbanUserInConversationTestThrowForbidden : IntegrationTestBase, IIntegrationTest
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
		await MessengerModule.RequestAsync(
			new JoinToChatCommand(
				RequesterId: bob.Value.Id,
				ChatId: conversation.Value.Id), CancellationToken.None);
		
		var banForAliceCommand = new BanUserInConversationCommand(
			RequesterId: user21Th.Value.Id,
			ChatId: conversation.Value.Id,
			UserId: alice.Value.Id,
			BanMinutes: 15);
		
		await MessengerModule.RequestAsync(banForAliceCommand, CancellationToken.None);
		
		var unbanUserAliceInConversationByBobCommand = new UnbanUserInConversationCommand(
			RequesterId: bob.Value.Id,
			ChatId: conversation.Value.Id,
			UserId: alice.Value.Id);
		
		var unbanUserAliceInConversationByBobResult = 
			await MessengerModule.RequestAsync(unbanUserAliceInConversationByBobCommand, CancellationToken.None);

		unbanUserAliceInConversationByBobResult.Error.Should().BeOfType<ForbiddenError>();
    }
}