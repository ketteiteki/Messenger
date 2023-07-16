using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Enums;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.RemoveUserFromConversationCommandHandlerTests;

public class RemoveUserFromConversationTestThrowForbidden : IntegrationTestBase, IIntegrationTest
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
		
		var addAliceInConversationCommand = new AddUserToConversationCommand(
			user21Th.Value.Id,
			createConversationResult.Value.Id,
			alice.Value.Id);
		
		var addBobInConversationCommand = new AddUserToConversationCommand(
			user21Th.Value.Id,
			createConversationResult.Value.Id,
			bob.Value.Id);

		await RequestAsync(addAliceInConversationCommand, CancellationToken.None);
		await RequestAsync(addBobInConversationCommand, CancellationToken.None);

		var removeAliceFromConversationByBobCommand = new RemoveUserFromConversationCommand(
			bob.Value.Id,
			createConversationResult.Value.Id,
			alice.Value.Id);

		var removeUserAlexFromConversationByBobResult = 
			await RequestAsync(removeAliceFromConversationByBobCommand, CancellationToken.None);

		removeUserAlexFromConversationByBobResult.Error.Should().BeOfType<ForbiddenError>();
    }
}