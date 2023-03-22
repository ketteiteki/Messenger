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

        var loginCommand = new LoginCommand(
            CommandHelper.Registration21ThCommand().Nickname,
            CommandHelper.Registration21ThCommand().Password,
            CommandHelper.Registration21ThCommand().Ip,
            CommandHelper.Registration21ThCommand().UserAgent);
        
        await MessengerModule.RequestAsync(loginCommand, CancellationToken.None);

        var getSessionListQuery = new GetSessionListQuery(user21Th.Value.Id);
        
        var getSessionListResult = await MessengerModule.RequestAsync(getSessionListQuery, CancellationToken.None);

        getSessionListResult.Value.Count.Should().Be(2);
    }
}