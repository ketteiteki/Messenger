using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Dialogs;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.CreateDialogCommandHandlerTests;

public class CreateDialogCommandHandlerTestSuccess : IntegrationTestBase, IIntegrationTest
{
	[Fact]
	public async Task Test()
	{
		var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
		var alice = await MessengerModule.RequestAsync(CommandHelper.RegistrationAliceCommand(), CancellationToken.None);
		
		var createDialogCommand = new CreateDialogCommand(
			RequesterId: user21Th.Value.Id,
			UserId: alice.Value.Id);

		var createDialogResult = await MessengerModule.RequestAsync(createDialogCommand, CancellationToken.None);

		createDialogResult.IsSuccess.Should().BeTrue();
		createDialogResult.Value.MembersCount.Should().Be(2);
		createDialogResult.Value.Members.Count.Should().Be(2);
		createDialogResult.Value.Members.Exists(m => m.Id == user21Th.Value.Id).Should().BeTrue();
		createDialogResult.Value.Members.Exists(m => m.Id != user21Th.Value.Id).Should().BeTrue();
		createDialogResult.Value.Type.Should().Be(ChatType.Dialog);
	}
}