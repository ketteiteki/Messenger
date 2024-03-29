using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Enums;
using Messenger.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiQueries.Dialogs;

public class GetDialogQueryHandler : IRequestHandler<GetDialogQuery, Result<ChatDto>>
{
	private readonly DatabaseContext _context;
	private readonly IBlobServiceSettings _blobServiceSettings;

	public GetDialogQueryHandler(
		DatabaseContext context,
		IBlobServiceSettings blobServiceSettings)
	{
		_context = context;
		_blobServiceSettings = blobServiceSettings;
	}

	public async Task<Result<ChatDto>> Handle(GetDialogQuery request, CancellationToken cancellationToken)
	{
		var dialog = await (
				from chatUser1 in _context.ChatUsers.AsNoTracking()
				join chatUser2 in _context.ChatUsers.AsNoTracking()
					on chatUser1.ChatId equals chatUser2.ChatId
				where chatUser1.Chat.Type == ChatType.Dialog 
				where chatUser1.UserId == request.RequesterId
				where chatUser2.UserId == request.UserId
				select new ChatDto
			{
				Id = chatUser2.Chat.Id,
				Name = chatUser2.Chat.Name,
				Title = chatUser2.Chat.Title,
				Type = chatUser2.Chat.Type,
				AvatarLink = chatUser2.User.AvatarFileName != null ? 
					$"{_blobServiceSettings.MessengerBlobAccess}/{chatUser2.User.AvatarFileName}" : null,
				LastMessageId = chatUser2.Chat.LastMessageId,
				LastMessageText = chatUser2.Chat.LastMessage != null ? chatUser2.Chat.LastMessage.Text : null,
				LastMessageAuthorDisplayName = chatUser2.Chat.LastMessage != null && chatUser2.Chat.LastMessage.Owner != null ? 
					chatUser2.Chat.LastMessage.Owner.DisplayName : null,
				LastMessageDateOfCreate = chatUser2.Chat.LastMessage != null ? chatUser2.Chat.LastMessage.DateOfCreate : null,
				MembersCount = 2,
				IsMember = true,
				CanSendMedia = true,
				Members = chatUser1.Chat.ChatUsers
					.Select(c => new UserDto(
						c.User.Id,
						c.User.DisplayName,
						c.User.Nickname,
						c.User.Bio,
						c.User.AvatarFileName != null ?
							$"{_blobServiceSettings.MessengerBlobAccess}/{c.User.AvatarFileName}" 
							: null)).ToList()
			})
			.FirstOrDefaultAsync(cancellationToken);

		if (dialog == null)
		{
			return new Result<ChatDto>(new DbEntityNotFoundError("Dialog not found")); 
		}

		return new Result<ChatDto>(dialog);
	}
}