using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Profiles;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.UpdateProfileDataCommandHandlerTests;

public class UpdateProfileDataTestSuccess : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);

        var updateProfileCommand = new UpdateProfileDataCommand(
            user21Th.Value.Id,
            DisplayName: "qwerty",
            Nickname: "liza",
            Bio: "i love your mom");
        
        var updatedProfileResult = await MessengerModule.RequestAsync(updateProfileCommand, CancellationToken.None);

        updatedProfileResult.Value.DisplayName.Should().Be(updateProfileCommand.DisplayName);
        updatedProfileResult.Value.Nickname.Should().Be(updateProfileCommand.Nickname);
        updatedProfileResult.Value.Bio.Should().Be(updateProfileCommand.Bio);
    }
}