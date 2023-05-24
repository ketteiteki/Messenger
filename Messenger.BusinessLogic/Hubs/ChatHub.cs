using Messenger.Domain.Constants;
using Messenger.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Messenger.BusinessLogic.Hubs;

[Authorize]
public class ChatHub : Hub<IChatHub>
{
    private readonly DatabaseContext _context;
    private readonly MemoryCache _memoryCache;
    private readonly MemoryCacheEntryOptions _memoryCacheEntryOptions;
    
    public ChatHub(DatabaseContext context, MemoryCache memoryCache)
    {
        _context = context;
        _memoryCache = memoryCache;
        _memoryCacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromSeconds(5))
            .SetAbsoluteExpiration(TimeSpan.FromSeconds(10));
    }

    public async Task JoinChat(Guid chatId)
    {
        var userId = Context.User?.Claims.First(x => x.Type == ClaimConstants.Id).Value;

        var isGettingEntriesSuccessful = _memoryCache.TryGetValue(userId, out List<UserChatEntry> userChatEntries);
        
        if (isGettingEntriesSuccessful)
        {
            var isChatInEntries = userChatEntries.Any(x => x.ChatId == chatId);
            
            if (isChatInEntries)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
                return;
            }
        }
        
        var userChats = await _context.ChatUsers
            .Where(x => x.UserId == new Guid(userId))
            .Select(x => new UserChatEntry(x.UserId, x.ChatId))
            .ToListAsync();

        _memoryCache.Set(userId, userChats, _memoryCacheEntryOptions);
        
        await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
    }
    
    public async Task LeaveChat(Guid chatId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId.ToString());
    }

    private record UserChatEntry(Guid UserId, Guid ChatId);
}