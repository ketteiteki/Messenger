using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Profiles;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.UpdateProfileAvatarCommandHandlerTests;

public class UpdateProfileAvatarTestSuccess : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);

        var updateProfileAvatarCommand = new UpdateProfileAvatarCommand(user21Th.Value.Id, FilesHelper.GetFile());
        
        var updateProfileAvatarResult = 
            await RequestAsync(updateProfileAvatarCommand, CancellationToken.None);

        updateProfileAvatarResult.IsSuccess.Should().BeTrue();
        updateProfileAvatarResult.Value.AvatarLink.Should().NotBeNull();
        
        var avatarFileName = updateProfileAvatarResult.Value.AvatarLink.Split("/")[^1];

        await BlobService.DeleteBlobAsync(avatarFileName);
    }
}