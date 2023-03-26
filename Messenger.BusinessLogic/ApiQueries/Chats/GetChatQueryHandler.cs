using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Enums;
using Messenger.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiQueries.Chats;

public class GetChatQueryHandler : IRequestHandler<GetChatQuery, Result<ChatDto>>
{
	private readonly DatabaseContext _context;
	private readonly IBlobServiceSettings _blobServiceSettings;

	public GetChatQueryHandler(
		DatabaseContext context,
		IBlobServiceSettings blobServiceSettings)
	{
		_context = context;
		_blobServiceSettings = blobServiceSettings;
	}

	public async Task<Result<ChatDto>> Handle(GetChatQuery request, CancellationToken cancellationToken)
	{
		var chatItem = 
			await (from chat in _context.Chats.AsNoTracking().Include(c => c.ChatUsers).ThenInclude(cu => cu.Role)
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
					where chat.Id == request.ChatId
					where chatUsersItem != null && chat.Type == ChatType.Dialog || 
					      chat.Type != ChatType.Dialog
					select new ChatDto
					{
						Id = chat.Id,
						Name = chat.Type != ChatType.Dialog ? chat.Name : null,
						Title = chat.Title,
						Type = chat.Type,
						AvatarLink = chat.Type != ChatType.Dialog ? chat.AvatarFileName != null ? 
							$"{_blobServiceSettings.MessengerBlobAccess}/{chat.AvatarFileName}" : null : null,
						LastMessageId = chat.LastMessageId,
						LastMessageText = chat.LastMessage != null ? chat.LastMessage.Text : null,
						LastMessageAuthorDisplayName = chat.LastMessage != null && chat.LastMessage.Owner != null ? 
							chat.LastMessage.Owner.DisplayName : null,
						LastMessageDateOfCreate = chat.LastMessage != null ? chat.LastMessage.DateOfCreate : null,
						MembersCount = chat.ChatUsers.Count,
						CanSendMedia = chatUsersItem != null && chatUsersItem.CanSendMedia,
						IsOwner = chat.OwnerId == request.RequesterId,
						IsMember = chatUsersItem != null,
						MuteDateOfExpire = chatUsersItem != null ? chatUsersItem.MuteDateOfExpire : null,
						BanDateOfExpire = banUserByChatItem != null ? banUserByChatItem.BanDateOfExpire : null,
						RoleUser = chatUsersItem != null && chatUsersItem.Role != null ?
							new RoleUserByChatDto(chatUsersItem.Role) : null,
						Members = chat.Type == ChatType.Dialog ? 
							chat.ChatUsers
								.Select(c => new UserDto(
									c.User.Id,
									c.User.DisplayName,
									c.User.Nickname,
									c.User.Bio,
									c.User.AvatarFileName != null ?
										$"{_blobServiceSettings.MessengerBlobAccess}/{c.User.AvatarFileName}" : null))
								.ToList() : new List<UserDto>(),
						UsersWithRole = chat.ChatUsers.Where(c => c.Role != null)
							.Select(cu => new RoleUserByChatDto(cu.Role)).ToList()
					})
				.FirstOrDefaultAsync(cancellationToken);

		if (chatItem == null)
		{
			return new Result<ChatDto>(new DbEntityNotFoundError("Chat not found"));
		}
		
		return new Result<ChatDto>(chatItem);
	}
}