using Messenger.BusinessLogic.ApiCommands.Dialogs;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.CreateDialogCommandHandlerTests;

public class CreateDialogCommandHandlerTestSuccess : IntegrationTestBase, IIntegrationTest
{
	[Fact]
	public async Task Test()
	{
		var user21th = EntityHelper.CreateUser21th();
		var alice = EntityHelper.CreateUserAlice();
		
		DatabaseContextFixture.Users.AddRange(user21th, alice);
		await DatabaseContextFixture.SaveChangesAsync();

		var createDialogCommand = new CreateDialogCommand(
			RequestorId: user21th.Id,
			UserId: alice.Id);

		await MessengerModule.RequestAsync(createDialogCommand, CancellationToken.None);
	}
}