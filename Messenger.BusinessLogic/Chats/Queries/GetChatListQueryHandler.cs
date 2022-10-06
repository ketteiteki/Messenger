using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.Domain.Enum;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.Chats.Queries;

public class GetChatListQueryHandler : IRequestHandler<GetChatListQuery, List<ChatDto>>
{
	private readonly DatabaseContext _context;

	public GetChatListQueryHandler(DatabaseContext context)
	{
		_context = context;
	}
	
	public async Task<List<ChatDto>> Handle(GetChatListQuery request, CancellationToken cancellationToken)
	{
		var chatList = 
			await (from chat in _context.Chats.AsNoTracking()
					join chatUsers in _context.ChatUsers.AsNoTracking()
						on new {x1 = request.RequesterId, x2 = chat.Id} equals new {x1 = chatUsers.UserId, x2 = chatUsers.ChatId }
						into chatUsersEnumerable
					from chatUsersItem in chatUsersEnumerable.DefaultIfEmpty()
					join banUserByChat in _context.BanUserByChats.AsNoTracking()
						on new {x1 = request.RequesterId, x2 = chat.Id} equals new {x1 = banUserByChat.UserId, x2 = banUserByChat.ChatId }
						into banUserByChatEnumerable
					from banUserByChatItem in banUserByChatEnumerable.DefaultIfEmpty()
					where chatUsersItem.UserId == request.RequesterId 
					select new ChatDto
					{
						Id = chat.Id,
						Name = (int)chat.Type != (int)ChatType.Dialog ? chat.Name : null,
						Title = chat.Title,
						Type = chat.Type,
						AvatarLink = (int)chat.Type != (int)ChatType.Dialog ? chat.AvatarLink : null,
						MembersCount = chat.ChatUsers.Count,
						CanSendMedia = chatUsersItem.CanSendMedia,
						IsOwner = chat.OwnerId == request.RequesterId,
						IsMember = chatUsersItem != null,
						MuteDateOfExpire = chatUsersItem != null ? chatUsersItem.MuteDateOfExpire : null,
						BanDateOfExpire = banUserByChatItem != null ? banUserByChatItem.BanDateOfExpire : null,
						RoleUser = chatUsersItem.Role != null ? new RoleUserByChatDto(chatUsersItem.Role) : null,
						Members = (int)chat.Type == (int)ChatType.Dialog ?
							chat.ChatUsers.Select(c => new UserDto(c.User)).ToList() : new()
					})
				.ToListAsync(cancellationToken);

		return chatList;
	}
}