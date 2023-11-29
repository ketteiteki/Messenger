using Messenger.Domain.Constants;
using Messenger.Domain.Entities;
using Messenger.Domain.Exceptions;
using Messenger.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Messenger.BusinessLogic.Services;

public class TicketStore : ITicketStore
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly TicketSerializer _ticketSerializer;
    private readonly IMemoryCache _memoryCache;
    private readonly MemoryCacheEntryOptions _memoryCacheEntryOptions;
    private readonly int _cookieExpireTimeSpan;

    public TicketStore(
        IServiceScopeFactory serviceScopeFactory,
        TicketSerializer ticketSerializer,
        IMemoryCache memoryCache, 
        int cookieExpireTimeSpan)
    {
        _ticketSerializer = ticketSerializer;
        _memoryCache = memoryCache;
        _cookieExpireTimeSpan = cookieExpireTimeSpan;
        _serviceScopeFactory = serviceScopeFactory;

        _memoryCacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromSeconds(15))
            .SetAbsoluteExpiration(TimeSpan.FromSeconds(100));
    }
    
    public async Task<string> StoreAsync(AuthenticationTicket ticket)
    {
        using var scope = _serviceScopeFactory.CreateScope(); 
        var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        
        var userId = ticket.Principal.Claims.First(x => x.Type == ClaimConstants.Id).Value;
        var sessionId = ticket.Principal.Claims.First(x => x.Type == ClaimConstants.SessionId).Value;

        var countUserSession = await context.UserSessions.Where(x => x.UserId == new Guid(userId)).CountAsync();
        var userSession = await context.UserSessions.FirstOrDefaultAsync(x => x.Id == new Guid(sessionId));

        if (countUserSession > 6)
        {
            var longUnusedUserSession = await context.UserSessions
                .Where(x => x.UserId == new Guid(userId))
                .OrderBy(x => x.DateOfLastAccess)
                .FirstAsync();

            context.UserSessions.Remove(longUnusedUserSession);
        }
        
        if (userSession != null)
        {
            userSession.UpdateExpiresAt(DateTimeOffset.UtcNow.AddMinutes(_cookieExpireTimeSpan));
            
            context.UserSessions.Update(userSession);
            await context.SaveChangesAsync();
            
            _memoryCache.Set(sessionId, ticket, _memoryCacheEntryOptions);
        }
        
        if (userSession == null)
        {
            var serializedTicket = _ticketSerializer.Serialize(ticket);
            
            var newUserSession = 
                new UserSessionEntity(new Guid(sessionId), new Guid(userId), DateTimeOffset.UtcNow.AddMinutes(_cookieExpireTimeSpan), serializedTicket);

            context.UserSessions.Add(newUserSession);
            await context.SaveChangesAsync();
            
            _memoryCache.Set(sessionId, ticket, _memoryCacheEntryOptions);
        }

        return sessionId;
    }

    public async Task RenewAsync(string key, AuthenticationTicket ticket)
    {
        using var scope = _serviceScopeFactory.CreateScope(); 
        var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        
        var userSession = await context.UserSessions.FirstOrDefaultAsync(x => x.Id == new Guid(key));

        if (userSession != null)
        {
            userSession.UpdateExpiresAt(DateTimeOffset.UtcNow.AddMinutes(_cookieExpireTimeSpan));
            
            context.UserSessions.Update(userSession);
            await context.SaveChangesAsync();
        }
    }

    public async Task<AuthenticationTicket> RetrieveAsync(string key)
    {
        using var scope = _serviceScopeFactory.CreateScope(); 
        var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        
        if (_memoryCache.TryGetValue<AuthenticationTicket>(key, out var ticket))
        {
            return ticket;
        }
        
        var userSession = await context.UserSessions.FirstOrDefaultAsync(x => x.Id == new Guid(key));

        if (userSession == null)
        {
            return null;
        }

        var deserializedTicket = _ticketSerializer.Deserialize(userSession.Value);
        
        if (deserializedTicket == null)
        {
            throw new StoreException("Deserialization ticket error");
        }
        
        userSession.UpdateDateOfLastAccess(DateTimeOffset.UtcNow);
            
        context.UserSessions.Update(userSession);
        await context.SaveChangesAsync();

        _memoryCache.Set(key, deserializedTicket, _memoryCacheEntryOptions);
        
        return deserializedTicket;
    }

    public async Task RemoveAsync(string key)
    {
        using var scope = _serviceScopeFactory.CreateScope(); 
        var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        
        var userSession = await context.UserSessions.FirstOrDefaultAsync(x => x.Id == new Guid(key));

        if (userSession == null)
        {
            return;
        }
        
        context.UserSessions.Remove(userSession);
        await context.SaveChangesAsync();
    }
}