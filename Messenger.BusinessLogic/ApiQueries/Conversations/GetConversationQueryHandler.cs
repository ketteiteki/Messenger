using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Enum;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiQueries.Conversations;

public class GetConversationQueryHandler : IRequestHandler<GetConversationQuery, Result<ChatDto>>
{
	private readonly DatabaseContext _context;

	public GetConversationQueryHandler(DatabaseContext context)
	{
		_context = context;
	}

	public async Task<Result<ChatDto>> Handle(GetConversationQuery request, CancellationToken cancellationToken)
	{
		var conversation = 
			await (from chat in _context.Chats.AsNoTracking()
					join chatUsers in _context.ChatUsers.AsNoTracking()
						on new {x1 = request.RequestorId, x2 = chat.Id} 
						equals new {x1 = chatUsers.UserId, x2 = chatUsers.ChatId }
						into chatUsersEnumerable
					from chatUsersItem in chatUsersEnumerable.DefaultIfEmpty()
					join banUserByChat in _context.BanUserByChats.AsNoTracking()
						on new {x1 = request.RequestorId, x2 = chat.Id} 
						equals new {x1 = banUserByChat.UserId, x2 = banUserByChat.ChatId }
						into banUserByChatEnumerable
					from banUserByChatItem in banUserByChatEnumerable.DefaultIfEmpty()
					where chat.Id == request.ChatId && chat.Type == ChatType.Сonversation
					select new ChatDto
					{
						Id = chat.Id,
						Name = chat.Name,
						Title = chat.Title,
						Type = chat.Type,
						AvatarLink = chat.AvatarLink,
						MembersCount = chat.ChatUsers.Count,
						CanSendMedia = chatUsersItem != null && chatUsersItem.CanSendMedia,
						IsOwner = chat.OwnerId == request.RequestorId,
						IsMember = chatUsersItem != null,
						MuteDateOfExpire = chatUsersItem != null ? chatUsersItem.MuteDateOfExpire : null,
						BanDateOfExpire = banUserByChatItem != null ? banUserByChatItem.BanDateOfExpire : null,
						RoleUser = chatUsersItem != null && chatUsersItem.Role != null ?
							new RoleUserByChatDto(chatUsersItem.Role) : null,
						Members = new()
					})
				.FirstOrDefaultAsync(cancellationToken);

		if (conversation == null) return new Result<ChatDto>(new DbEntityNotFoundError("Conversation not found"));

		return new Result<ChatDto>(conversation);
	}
}