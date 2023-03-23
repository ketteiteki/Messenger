using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Profiles;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.UpdateProfileAvatarCommandHandlerTests;

public class DeleteProfileAvatarTestSuccess : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);

        var firstUpdateProfileAvatarCommand = new UpdateProfileAvatarCommand(
            user21Th.Value.Id,
            FilesHelper.GetFile());
        
        var firstUpdateProfileAvatarResult = 
            await MessengerModule.RequestAsync(firstUpdateProfileAvatarCommand, CancellationToken.None);

        var secondUpdateProfileAvatarCommand = new UpdateProfileAvatarCommand(
            user21Th.Value.Id,
            AvatarFile: null);
        
        var secondUpdateProfileAvatarResult = 
            await MessengerModule.RequestAsync(secondUpdateProfileAvatarCommand, CancellationToken.None);

        firstUpdateProfileAvatarResult.Value.AvatarLink.Should().NotBeNull();
        secondUpdateProfileAvatarResult.IsSuccess.Should().BeTrue();
    }
}