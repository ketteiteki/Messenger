using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiCommands.Profiles;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.DeleteProfileCommandHandlerTests;

public class DeleteProfileTestSuccess : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);

        await MessengerModule.RequestAsync(new CreateChatCommand(
            RequesterId: user21Th.Value.Id,
            Name: "qwerty",
            Title: "qwerty",
            Type: ChatType.Conversation,
            AvatarFile: null), CancellationToken.None);
        
        var deletedProfile = await MessengerModule.RequestAsync(new DeleteProfileCommand(
            RequesterId: user21Th.Value.Id), CancellationToken.None);

        deletedProfile.IsSuccess.Should().BeTrue();
    }
}