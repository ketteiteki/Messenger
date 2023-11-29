using Messenger.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Messenger.BusinessLogic.Services;

public class RefreshService : IHostedService
{
    private readonly DatabaseContext _context;
    private readonly TicketStore _ticketStore;
    private readonly TicketSerializer _ticketSerializer;
    private readonly int _timeIntervalForCheckingExpiredTickets;


    public RefreshService(
        DatabaseContext context,
        TicketStore ticketStore,
        int timeIntervalForCheckingExpiredTickets)
    {
        _context = context;
        _ticketStore = ticketStore;
        _timeIntervalForCheckingExpiredTickets = timeIntervalForCheckingExpiredTickets;

        _ticketSerializer = new TicketSerializer();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var _ = StartRefreshingUserSessionsAsync(cancellationToken);
        
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task StartRefreshingUserSessionsAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var expiringUserSessions = await _context.UserSessions
                .Where(x => (x.ExpiresAt > DateTimeOffset.UtcNow &&
                             x.ExpiresAt < DateTimeOffset.UtcNow.AddMinutes(_timeIntervalForCheckingExpiredTickets)) ||
                            x.ExpiresAt < DateTimeOffset.UtcNow)
                .ToListAsync(cancellationToken);

            foreach (var userSession in expiringUserSessions)
            {
                var differenceBetweenLastAccessAndUtcNow = userSession.DateOfLastAccess
                    .Subtract(DateTimeOffset.UtcNow)
                    .Duration();
                
                if (differenceBetweenLastAccessAndUtcNow > TimeSpan.FromDays(3))
                {
                    await _ticketStore.RemoveAsync(userSession.Id.ToString());
                    continue;
                }
                
                var deserializedTicket = _ticketSerializer.Deserialize(userSession.Value);
                
                await _ticketStore.RenewAsync(userSession.Id.ToString(), deserializedTicket);
            }

            await Task.Delay(TimeSpan.FromMinutes(_timeIntervalForCheckingExpiredTickets), cancellationToken);
        }
    }
}