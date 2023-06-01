using Messenger.Domain.Constants;
using Messenger.Domain.Entities;
using Messenger.Domain.Exceptions;
using Messenger.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Messenger.BusinessLogic.Services;

public class TicketStore : ITicketStore
{
    private readonly DatabaseContext _context;
    private readonly TicketSerializer _ticketSerializer;
    private readonly IMemoryCache _memoryCache;
    private readonly MemoryCacheEntryOptions _memoryCacheEntryOptions;
    private readonly int _cookieExpireTimeSpan;

    public TicketStore(
        DatabaseContext context,
        TicketSerializer ticketSerializer,
        IMemoryCache memoryCache, 
        int cookieExpireTimeSpan)
    {
        _context = context;
        _ticketSerializer = ticketSerializer;
        _memoryCache = memoryCache;
        _cookieExpireTimeSpan = cookieExpireTimeSpan;

        _memoryCacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromSeconds(15));
    }
    
    public async Task<string> StoreAsync(AuthenticationTicket ticket)
    {
        var userId = ticket.Principal.Claims.First(x => x.Type == ClaimConstants.Id).Value;
        var sessionId = ticket.Principal.Claims.First(x => x.Type == ClaimConstants.SessionId).Value;
      
        var userSession = await _context.UserSessions.FirstOrDefaultAsync(x => x.Id == new Guid(sessionId));

        var ticketExpiresUtc = ticket.Properties.ExpiresUtc;
        
        if (ticketExpiresUtc.HasValue == false)
        {
            throw new StoreException("Ticket ExpiresUtc value does not exist");
        }

        if (userSession != null)
        {
            userSession.UpdateExpiresAt(DateTimeOffset.UtcNow.AddMinutes(_cookieExpireTimeSpan));
            
            _context.UserSessions.Update(userSession);
            await _context.SaveChangesAsync();
            
            _memoryCache.Set(sessionId, ticket, _memoryCacheEntryOptions);
        }
        
        if (userSession == null)
        {
            var serializedTicket = _ticketSerializer.Serialize(ticket);
            
            var newUserSession = 
                new UserSessionEntity(new Guid(sessionId), new Guid(userId), ticketExpiresUtc.Value, serializedTicket);

            _context.UserSessions.Add(newUserSession);
            await _context.SaveChangesAsync();
            
            _memoryCache.Set(sessionId, ticket, _memoryCacheEntryOptions);
        }

        return sessionId;
    }

    public async Task RenewAsync(string key, AuthenticationTicket ticket)
    {
        var userSession = await _context.UserSessions.FirstOrDefaultAsync(x => x.Id == new Guid(key));

        if (userSession != null)
        {
            userSession.UpdateExpiresAt(DateTimeOffset.UtcNow.AddMinutes(_cookieExpireTimeSpan));
            
            _context.UserSessions.Update(userSession);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<AuthenticationTicket> RetrieveAsync(string key)
    {
        if (_memoryCache.TryGetValue<AuthenticationTicket>(key, out var ticket))
        {
            return ticket;
        }
        
        var userSession = await _context.UserSessions.FirstOrDefaultAsync(x => x.Id == new Guid(key));

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
            
        _context.UserSessions.Update(userSession);
        await _context.SaveChangesAsync();

        _memoryCache.Set(key, deserializedTicket);
        
        return deserializedTicket;
    }

    public async Task RemoveAsync(string key)
    {
        var userSession = await _context.UserSessions.FirstAsync(x => x.Id == new Guid(key));
        
        _context.UserSessions.Remove(userSession);
        await _context.SaveChangesAsync();
    }
}