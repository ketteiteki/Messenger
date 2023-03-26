using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Persistence;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiQueries.Users;

public class GetUserListByChatQueryHandler : IRequestHandler<GetUserListByChatQuery, Result<List<UserDto>>>
{
    private readonly DatabaseContext _context;
    private readonly IBlobServiceSettings _blobServiceSettings;

    public GetUserListByChatQueryHandler(
        DatabaseContext context,
        IBlobServiceSettings blobServiceSettings)
    {
        _context = context;
        _blobServiceSettings = blobServiceSettings;
    }

    public async Task<Result<List<UserDto>>> Handle(GetUserListByChatQuery request, CancellationToken cancellationToken)
    {
        if (request.Limit > 40)
        {
            return new Result<List<UserDto>>(new BadRequestError("Limit must be no more than 40"));
        }
        
        if (request.Page <= 0)
        {
            return new Result<List<UserDto>>(new BadRequestError("Page must be greater than 0"));
        }

        var banRequesterByChat = await _context.BanUserByChats
            .AsNoTracking()
            .FirstOrDefaultAsync(b => 
                b.UserId == request.RequesterId &&
                b.ChatId == request.RequesterId, cancellationToken);

        if (banRequesterByChat != null && banRequesterByChat.BanDateOfExpire > DateTime.UtcNow)
        {
            return new Result<List<UserDto>>(
                new ForbiddenError($"You are banned in the chat. Unban date: {banRequesterByChat.BanDateOfExpire}"));
        }

        var userList = await _context.ChatUsers
            .AsNoTracking()
            .Where(c => c.ChatId == request.ChatId)
            .Skip(request.Limit * (request.Page - 1))
            .Take(request.Limit)
            .Select(u => new UserDto(
                u.User.Id,
                u.User.DisplayName,
                u.User.Nickname,
                u.User.Bio,
                u.User.AvatarFileName != null ? 
                    $"{_blobServiceSettings.MessengerBlobAccess}/{u.User.AvatarFileName}" : null))
            .ToListAsync(cancellationToken);

        return new Result<List<UserDto>>(userList);
    }
}