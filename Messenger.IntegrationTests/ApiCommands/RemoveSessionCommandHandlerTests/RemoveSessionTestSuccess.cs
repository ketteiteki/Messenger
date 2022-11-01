using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Auth;
using Messenger.BusinessLogic.ApiQueries.Auth;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.RemoveSessionCommandHandlerTests;

public class RemoveSessionTestSuccess : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await MessengerModule.RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);

        const string loginIp = "432.64.879.989";
        
        await MessengerModule.RequestAsync(new LoginCommand(
            Nickname: CommandHelper.Registration21ThCommand().Nickname,
            Password: CommandHelper.Registration21ThCommand().Password,
            Ip: loginIp,
            UserAgent: CommandHelper.Registration21ThCommand().UserAgent), CancellationToken.None);

        var firstGetSessionList = await MessengerModule.RequestAsync(new GetSessionListQuery(
            RequesterId: user21Th.Value.Id), CancellationToken.None);

        var firstCreatedSession = firstGetSessionList.Value.DistinctBy(s => s.CreateAt).First();

        await MessengerModule.RequestAsync(new RemoveSessionCommand(
            RequesterId: user21Th.Value.Id,
            SessionId: firstCreatedSession.Id), CancellationToken.None);
        
        var secondGetSessionList = await MessengerModule.RequestAsync(new GetSessionListQuery(
            RequesterId: user21Th.Value.Id), CancellationToken.None);

        secondGetSessionList.Value.Count.Should().Be(1);
        
        secondGetSessionList.Value.First().Ip.Should().Be(loginIp);
    }
}