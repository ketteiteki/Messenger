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

        var createConversationCommand = new CreateChatCommand(
            user21Th.Value.Id,
            Name: "qwerty",
            Title: "qwerty",
            ChatType.Conversation,
            AvatarFile: null);
        
        await MessengerModule.RequestAsync(createConversationCommand, CancellationToken.None);

        var deleteProfileCommand = new DeleteProfileCommand(user21Th.Value.Id);
        
        var deletedProfile = await MessengerModule.RequestAsync(deleteProfileCommand, CancellationToken.None);

        deletedProfile.IsSuccess.Should().BeTrue();
    }
}