using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Auth;
using Messenger.Domain.Entities;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.TerminateSessionCommandHandlerTests;

public class TerminateSessionTestSuccess : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var user21Th = await RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);

        var userSessionGuid = Guid.NewGuid();
        
        var userSession = new UserSessionEntity(userSessionGuid, user21Th.Value.Id, DateTimeOffset.UtcNow, Array.Empty<byte>());

        DatabaseContextFixture.UserSessions.Add(userSession);
        await DatabaseContextFixture.SaveChangesAsync();

        var terminateSessionCommand = new TerminateSessionCommand(user21Th.Value.Id, userSessionGuid);
        
        var terminateSessionResult = await RequestAsync(terminateSessionCommand, CancellationToken.None);

        terminateSessionResult.IsSuccess.Should().BeTrue();
    }
}