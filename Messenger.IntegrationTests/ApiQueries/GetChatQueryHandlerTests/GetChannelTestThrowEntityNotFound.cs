using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Chats;
using Messenger.BusinessLogic.ApiQueries.Chats;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Enum;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiQueries.GetChatQueryHandlerTests;

public class GetChannelTestThrowEntityNotFound : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
        var bob = await MessengerModule.RequestAsync(CommandHelper.RegistrationBobCommand(), CancellationToken.None);

        var createChannelCommand = new CreateChatCommand(
            RequesterId: user21Th.Value.Id,
            Name: "qwerty",
            Title: "qwerty",
            Type: ChatType.Channel,
            AvatarFile: null);

        await MessengerModule.RequestAsync(createChannelCommand, CancellationToken.None);

        var getChannelByBobQuery = new GetChatQuery(
            RequesterId: bob.Value.Id,
            ChatId: new Guid());

        var getChannelByBobResult = await MessengerModule.RequestAsync(getChannelByBobQuery, CancellationToken.None);

        getChannelByBobResult.Error.Should().BeOfType<DbEntityNotFoundError>();
    }
}