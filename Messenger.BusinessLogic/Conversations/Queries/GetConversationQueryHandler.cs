// using MediatR;
// using Messenger.Application.Exceptions;
// using Messenger.Domain.Common.DTOs;
// using Messenger.Domain.Enum;
// using Messenger.Services;
// using Microsoft.EntityFrameworkCore;
//
// namespace Messenger.Application.Conversations.Queries;
//
// public class GetConversationQueryHandler : IRequestHandler<GetConversationQuery, ChatDto>
// {
// 	private readonly DatabaseContext _context;
//
// 	private GetConversationQueryHandler(DatabaseContext context)
// 	{
// 		_context = context;
// 	}
// 	
// 	public async Task<ChatDto> Handle(GetConversationQuery request, CancellationToken cancellationToken)
// 	{
// 		var conversation = await (
// 				from chat in _context.Chats.AsNoTracking()
// 				join chatUser in _context.ChatUsers.AsNoTracking()
// 					on request.ChatId equals chatUser.ChatId
// 					into chatUsersEnumerable
// 				join banUserByChat in _context.BanUserByChats.AsNoTracking()
// 					on new { x1 = request.RequesterId, x2 = request.ChatId }
// 					equals new { x1 = banUserByChat.UserId, x2 = banUserByChat.ChatId }
// 					into banUserByChatEnumerable
// 				from chatUsersItem in chatUsersEnumerable.DefaultIfEmpty()
// 				from banUserByChatItem in banUserByChatEnumerable.DefaultIfEmpty()
// 				where chat.Id == request.ChatId && chat.Type == ChatType.Сonversation
// 				select new ChatDto
// 				{
// 					Id = chat.Id,
// 					Name = chat.Name,
// 					Title = chat.Title,
// 					Type = chat.Type,
// 					AvatarLink = chat.AvatarLink,
// 					MembersCount = chat.ChatUsers.Count,
// 					CanSendMedia = chatUsersItem.CanSendMedia,
// 					IsOwner = chat.OwnerId == request.RequesterId,
// 					IsMember = chatUsersItem != null,
// 					MuteDateOfExpire = chatUsersItem != null ? chatUsersItem.MuteDateOfExpire : null,
// 					BanDateOfExpire = banUserByChatItem != null ? banUserByChatItem.BanDateOfExpire : null,
// 					RoleUser = chatUsersItem.Role != null ? new RoleUserByChatDto(chatUsersItem.Role) : null
// 				})
// 			.FirstOrDefaultAsync(cancellationToken);
//
// 		if (conversation == null) throw new DbEntityNotFoundException("Conversation not found");
//
// 		return conversation;
// 	}
// }