using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Auth;
using Messenger.BusinessLogic.ApiQueries.Auth;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiQueries.GetSessionListQueryHandlerTests;

public class GetSessionListTestSuccess : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);
        
        await MessengerModule.RequestAsync(new LoginCommand(
            Nickname: CommandHelper.Registration21ThCommand().Nickname,
            Password: CommandHelper.Registration21ThCommand().Password,
            Ip: CommandHelper.Registration21ThCommand().Ip,
            UserAgent: CommandHelper.Registration21ThCommand().UserAgent), CancellationToken.None);

        var getSessionListResult = await MessengerModule.RequestAsync(new GetSessionListQuery(
            RequesterId: user21Th.Value.Id), CancellationToken.None);

        getSessionListResult.Value.Count.Should().Be(2);
    }
}