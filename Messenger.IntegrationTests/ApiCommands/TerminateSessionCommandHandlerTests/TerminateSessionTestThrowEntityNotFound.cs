using FluentAssertions;
using Messenger.BusinessLogic.ApiCommands.Auth;
using Messenger.BusinessLogic.Responses;
using Messenger.IntegrationTests.Abstraction;
using Messenger.IntegrationTests.Helpers;
using Xunit;

namespace Messenger.IntegrationTests.ApiCommands.TerminateSessionCommandHandlerTests;

public class TerminateSessionTestThrowEntityNotFound : IntegrationTestBase, IIntegrationTest
{
    [Fact]
    public async Task Test()
    {
        var firstTerminateSessionCommand = new TerminateSessionCommand(Guid.NewGuid(), Guid.NewGuid());
        var firstTerminateSessionResult = await RequestAsync(firstTerminateSessionCommand, CancellationToken.None);
        
        firstTerminateSessionResult.Error.Should().BeOfType<DbEntityNotFoundError>();
        
        var user21Th = await RequestAsync(CommandHelper.Registration21ThCommand(), CancellationToken.None);

        var secondTerminateSessionCommand = new TerminateSessionCommand(user21Th.Value.Id, Guid.NewGuid());
        var secondTerminateSessionResult = await RequestAsync(secondTerminateSessionCommand, CancellationToken.None);

        secondTerminateSessionResult.Error.Should().BeOfType<DbEntityNotFoundError>();
    }
}