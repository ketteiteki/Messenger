using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Conversations;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Enums;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.AddUserToConversationCommandHandlerTests;

public class AddUserToConversationTestThrowEntityExists : IntegrationTestBase, IIntegrationTest
{
	[Fact]
    public async Task Test()
    {
        var user21Th = await RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
		var alice = await RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);

		var createConversationCommand = new CreateChatCommand(
			user21Th.Value.Id,
			Name: "qwerty",
			Title: "qwerty",
			ChatType.Conversation,
			AvatarFile: null);
		
		var createConversationResult = await RequestAsync(createConversationCommand, CancellationToken.None);

		var addUserAliceToConversationBy21ThCommand = new AddUserToConversationCommand(
			user21Th.Value.Id,
			createConversationResult.Value.Id,
			alice.Value.Id);
		
	    await RequestAsync(addUserAliceToConversationBy21ThCommand, CancellationToken.None);
		
		var secondAddUserAliceToConversationBy21ThResult = 
			await RequestAsync(addUserAliceToConversationBy21ThCommand, CancellationToken.None);

		secondAddUserAliceToConversationBy21ThResult.Error.Should().BeOfType<DbEntityExistsError>();
    }
}