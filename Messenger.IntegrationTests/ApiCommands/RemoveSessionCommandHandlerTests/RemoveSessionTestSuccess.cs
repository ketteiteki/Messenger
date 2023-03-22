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
        
        var loginCommand = new LoginCommand(
            CommandHelper.Registration21ThCommand().Nickname,
            CommandHelper.Registration21ThCommand().Password,
            loginIp,
            CommandHelper.Registration21ThCommand().UserAgent);
        
        await MessengerModule.RequestAsync(loginCommand, CancellationToken.None);

        var firstGetSessionListQuery = new GetSessionListQuery(user21Th.Value.Id);
        
        var firstGetSessionListResult = await MessengerModule.RequestAsync(firstGetSessionListQuery, CancellationToken.None);

        var firstCreatedSession = firstGetSessionListResult.Value.DistinctBy(s => s.CreateAt).First();

        var removeFirstCreatedSessionCommand = new RemoveSessionCommand(user21Th.Value.Id, firstCreatedSession.Id); 
        
        await MessengerModule.RequestAsync(removeFirstCreatedSessionCommand, CancellationToken.None);

        var secondGetSessionListQuery = new GetSessionListQuery(user21Th.Value.Id);
        
        var secondGetSessionListResult = await MessengerModule.RequestAsync(secondGetSessionListQuery, CancellationToken.None);

        secondGetSessionListResult.Value.Count.Should().Be(1);
        secondGetSessionListResult.Value.First().Ip.Should().Be(loginIp);
    }
}