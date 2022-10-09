using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiQueries.Messages;

public class GetMessageListQueryHandler : IRequestHandler<GetMessageListQuery, Result<List<MessageDto>>>
{
	private readonly DatabaseContext _context;

	public GetMessageListQueryHandler(DatabaseContext context)
	{
		_context = context;
	}
	
	public async Task<Result<List<MessageDto>>> Handle(GetMessageListQuery request, CancellationToken cancellationToken)
	{
		var banUserByChat = await _context.BanUserByChats
			.FirstOrDefaultAsync(b => b.UserId == request.RequestorId && b.ChatId == request.ChatId, cancellationToken);

		if (banUserByChat != null) return new Result<List<MessageDto>>(new ForbiddenError("You are banned"));
		
		if (request.FromMessageDateTime != null)
		{
			var messagesFromMessageId = await _context.Messages
				.AsNoTracking()
				.Where(m => m.ChatId == request.ChatId 
				            && m.DateOfCreate < request.FromMessageDateTime)
				.OrderBy(m => m.DateOfCreate)
				.Take(request.Limit)
				.Select(m => new MessageDto
				{
					Id = m.Id,
					Text = m.Text,
					IsEdit = m.IsEdit,
					OwnerId = m.OwnerId,
					OwnerDisplayName = m.Owner != null ? m.Owner.DisplayName : null,
					OwnerAvatarLink = m.Owner != null ? m.Owner.AvatarLink : null,
					ReplyToMessageId = m.ReplyToMessageId,
					ReplyToMessageText = m.ReplyToMessage != null ? m.ReplyToMessage.Text : null,
					ReplyToMessageAuthorDisplayName = m.ReplyToMessage != null && m.ReplyToMessage.Owner != null ? 
						m.ReplyToMessage.Owner.DisplayName : null,
					Attachments = m.Attachments.Select(a => new AttachmentDto(a)).ToList(),
					ChatId = m.ChatId,
					DateOfCreate = m.DateOfCreate
				})
				.ToListAsync(cancellationToken);
			
			return new Result<List<MessageDto>>(messagesFromMessageId);
		}
		
		var messages = await _context.Messages
			.AsNoTracking()
			.Where(m => m.ChatId == request.ChatId)
			.OrderBy(m => m.DateOfCreate)
			.Take(request.Limit)
			.Select(m => new MessageDto
			{
				Id = m.Id,
				Text = m.Text,
				IsEdit = m.IsEdit,
				OwnerId = m.OwnerId,
				OwnerDisplayName = m.Owner != null ? m.Owner.DisplayName : null,
				OwnerAvatarLink = m.Owner != null ? m.Owner.AvatarLink : null,
				ReplyToMessageId = m.ReplyToMessageId,
				ReplyToMessageText = m.ReplyToMessage != null ? m.ReplyToMessage.Text : null,
				ReplyToMessageAuthorDisplayName = m.ReplyToMessage != null && m.ReplyToMessage.Owner != null ? 
					m.ReplyToMessage.Owner.DisplayName : null,
				Attachments = m.Attachments.Select(a => new AttachmentDto(a)).ToList(),
				ChatId = m.ChatId,
				DateOfCreate = m.DateOfCreate
			})
			.ToListAsync(cancellationToken);
			
		return new Result<List<MessageDto>>(messages);
	}
}
