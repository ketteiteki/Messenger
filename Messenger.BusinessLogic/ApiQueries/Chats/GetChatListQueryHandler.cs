using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Enum;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiQueries.Chats;

public class GetChatListQueryHandler : IRequestHandler<GetChatListQuery, Result<List<ChatDto>>>
{
	private readonly DatabaseContext _context;

	public GetChatListQueryHandler(DatabaseContext context)
	{
		_context = context;
	}
	
	public async Task<Result<List<ChatDto>>> Handle(GetChatListQuery request, CancellationToken cancellationToken)
	{
		var chatList = 
			await (from chat in _context.Chats.AsNoTracking()
					join chatUsers in _context.ChatUsers.AsNoTracking()
						on new {x1 = request.RequesterId, x2 = chat.Id} 
						equals new {x1 = chatUsers.UserId, x2 = chatUsers.ChatId }
						into chatUsersEnumerable
					from chatUsersItem in chatUsersEnumerable.DefaultIfEmpty()
					join deletedDialogByUsers in _context.DeletedDialogByUsers.AsNoTracking()
						on new {x1 = chatUsersItem.UserId, x2 = chatUsersItem.ChatId} 
						equals new {x1 = deletedDialogByUsers.UserId, x2 = deletedDialogByUsers.ChatId }
						into deletedDialogByUsersEnumerable
					from deletedDialogByUsersItem in deletedDialogByUsersEnumerable.DefaultIfEmpty()
					join banUserByChat in _context.BanUserByChats.AsNoTracking()
						on new {x1 = request.RequesterId, x2 = chat.Id} 
						equals new {x1 = banUserByChat.UserId, x2 = banUserByChat.ChatId }
						into banUserByChatEnumerable
					from banUserByChatItem in banUserByChatEnumerable.DefaultIfEmpty()
					where chatUsersItem.UserId == request.RequesterId
					where deletedDialogByUsersItem == null
					select new ChatDto
					{
						Id = chat.Id,
						Name = chat.Type != ChatType.Dialog ? chat.Name : null,
						Title = chat.Title,
						Type = chat.Type,
						AvatarLink = chat.Type != ChatType.Dialog ? chat.AvatarLink : null,
						LastMessageId = chat.LastMessageId,
						LastMessageText = chat.LastMessage != null ? chat.LastMessage.Text : null,
						LastMessageAuthorDisplayName = chat.LastMessage != null && chat.LastMessage.Owner != null ? 
							chat.LastMessage.Owner.DisplayName : null,
						LastMessageDateOfCreate = chat.LastMessage != null ? chat.LastMessage.DateOfCreate : null,
						MembersCount = chat.ChatUsers.Count,
						CanSendMedia = chatUsersItem.CanSendMedia,
						IsOwner = chat.OwnerId == request.RequesterId,
						IsMember = chatUsersItem != null,
						MuteDateOfExpire = chatUsersItem != null ? chatUsersItem.MuteDateOfExpire : null,
						BanDateOfExpire = banUserByChatItem != null ? banUserByChatItem.BanDateOfExpire : null,
						RoleUser = chatUsersItem.Role != null ? new RoleUserByChatDto(chatUsersItem.Role) : null,
						Members = chat.Type == ChatType.Dialog ?
							chat.ChatUsers.Select(c => new UserDto(c.User)).ToList() : new List<UserDto>(),
					})
				.ToListAsync(cancellationToken);

		return new Result<List<ChatDto>>(chatList);
	}
}
