using System.Text.RegularExpressions;
using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Enum;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiQueries.Chats;

public class GetChatListBySearchQueryHandler : IRequestHandler<GetChatListBySearchQuery, Result<List<ChatDto>>>
{
    private readonly DatabaseContext _context;

    public GetChatListBySearchQueryHandler(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<Result<List<ChatDto>>> Handle(GetChatListBySearchQuery request, CancellationToken cancellationToken)
    {
        var chatList = 
            await (from chat in _context.Chats.AsNoTracking()
                    join chatUsers in _context.ChatUsers.AsNoTracking()
                        on new {x1 = request.RequesterId, x2 = chat.Id} 
                        equals new {x1 = chatUsers.UserId, x2 = chatUsers.ChatId }
                        into chatUsersEnumerable
                    from chatUsersItem in chatUsersEnumerable.DefaultIfEmpty()
                    join banUserByChat in _context.BanUserByChats.AsNoTracking()
                        on new {x1 = request.RequesterId, x2 = chat.Id} 
                        equals new {x1 = banUserByChat.UserId, x2 = banUserByChat.ChatId }
                        into banUserByChatEnumerable
                    from banUserByChatItem in banUserByChatEnumerable.DefaultIfEmpty()
                    where chat.Type != ChatType.Dialog
                    where Regex.IsMatch(chat.Title, $"{request.SearchText}") ||
                          Regex.IsMatch(chat.Name, $"{request.SearchText}")
                    select new ChatDto
                    {
                        Id = chat.Id,
                        Name = chat.Name,
                        Title = chat.Title,
                        Type = chat.Type,
                        AvatarLink = chat.AvatarLink,
                        MembersCount = chat.ChatUsers.Count,
                        CanSendMedia = chatUsersItem != null && chatUsersItem.CanSendMedia,
                        IsOwner = chat.OwnerId == request.RequesterId,
                        IsMember = chatUsersItem != null,
                        MuteDateOfExpire = chatUsersItem != null ? chatUsersItem.MuteDateOfExpire : null,
                        BanDateOfExpire = banUserByChatItem != null ? banUserByChatItem.BanDateOfExpire : null,
                        RoleUser = chatUsersItem.Role != null ? new RoleUserByChatDto(chatUsersItem.Role) : null,
                        Members = new List<UserDto>()
                    })
                .ToListAsync(cancellationToken);

        return new Result<List<ChatDto>>(chatList);
    }
}