using Messenger.BusinessLogic.Dialogs.Command;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.Dialogs;

public class CreateDialogCommandHandlerTestSuccess : IntegrationTestBase, IIntegrationTest
{
	[Fact]
	public async Task Test()
	{
		var user21th = EntityHelper.CreateUser21th();
		var alice = EntityHelper.CreateUserAlice();
		
		DatabaseContextFixture.Users.AddRange(user21th, alice);
		await DatabaseContextFixture.SaveChangesAsync();

		var createDialogCommand = new CreateDialogCommand
		{
			RequesterId = user21th.Id,
			UserId = alice.Id
		};

		await MessengerModule.RequestAsync(createDialogCommand, CancellationToken.None);
	}
}