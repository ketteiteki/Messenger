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
            RequesterId: user21Th.Value.Id,
            DisplayName: "qwerty",
            NickName: "zxcvbn",
            Bio: "i love your mom");
        
        var updatedProfile = await MessengerModule.RequestAsync(updateProfileCommand, CancellationToken.None);

        updatedProfile.Value.DisplayName.Should().Be(updateProfileCommand.DisplayName);
        updatedProfile.Value.Nickname.Should().Be(updateProfileCommand.NickName);
        updatedProfile.Value.Bio.Should().Be(updateProfileCommand.Bio);
    }
}