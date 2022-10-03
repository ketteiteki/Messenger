using MediatR;
using Messenger.BusinessLogic.Exceptions;
using Messenger.BusinessLogic.Models;
using Messenger.Domain.Enum;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.Chats.Queries;

public class GetChatQueryHandler : IRequestHandler<GetChatQuery, ChatDto>
{
	private readonly DatabaseContext _context;

	public GetChatQueryHandler(DatabaseContext context)
	{
		_context = context;
	}

	public async Task<ChatDto> Handle(GetChatQuery request, CancellationToken cancellationToken)
	{
		var chatItem = 
			await (from chat in _context.Chats.AsNoTracking()
					join chatUsers in _context.ChatUsers.AsNoTracking()
						on new {x1 = request.RequesterId, x2 = chat.Id} equals new {x1 = chatUsers.UserId,x2 = chatUsers.ChatId }
						into chatUsersEnumerable
					join banUserByChat in _context.BanUserByChats.AsNoTracking()
						on new {x1 = request.RequesterId, x2 = chat.Id} equals new {x1 = banUserByChat.UserId,x2 = banUserByChat.ChatId }
						into banUserByChatEnumerable
					from banUserByChatItem in banUserByChatEnumerable.DefaultIfEmpty()
					from chatUsersItem in chatUsersEnumerable.DefaultIfEmpty()
					where chatUsersItem.UserId == request.RequesterId &&
					      (chat.Type != ChatType.Dialog || chat.Messages.Count != 0)
					select new ChatDto
					{
						Id = chat.Id,
						Name = chat.Type == ChatType.Dialog ? 
							chat.ChatUsers.Find(u => u.User.Id != request.RequesterId).User.NickName : 
							chat.Name,
						Title = chat.Title,
						Type = chat.Type,
						AvatarLink =  chat.Type == ChatType.Dialog ? 
							chat.ChatUsers.Find(u => u.User.Id != request.RequesterId).User.AvatarLink : 
							chat.AvatarLink,
						MembersCount = chat.ChatUsers.Count,
						CanSendMedia = chatUsersItem.CanSendMedia,
						IsOwner = chat.OwnerId == request.RequesterId,
						IsMember = chatUsersItem != null,
						MuteDateOfExpire = chatUsersItem != null ? chatUsersItem.MuteDateOfExpire : null,
						BanDateOfExpire = banUserByChatItem != null ? banUserByChatItem.BanDateOfExpire : null,
						RoleUser = chatUsersItem.Role != null ? new RoleUserByChatDto(chatUsersItem.Role) : null
					})
				.FirstOrDefaultAsync(cancellationToken);

		if (chatItem == null) throw new DbEntityNotFoundException("Chat not found");
		
		return chatItem;
	}
}