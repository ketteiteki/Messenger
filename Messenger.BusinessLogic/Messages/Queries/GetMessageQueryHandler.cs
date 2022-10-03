using MediatR;
using Messenger.BusinessLogic.Exceptions;
using Messenger.BusinessLogic.Models;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.Messages.Queries;

public class GetMessageQueryHandler : IRequestHandler<GetMessageQuery, MessageDto>
{
	private readonly DatabaseContext _context;

	public GetMessageQueryHandler(DatabaseContext context)
	{
		_context = context;
	}
	
	public async Task<MessageDto> Handle(GetMessageQuery request, CancellationToken cancellationToken)
	{
		var banUserByChat = await _context.BanUserByChats
			.FirstOrDefaultAsync(b => b.UserId == request.RequesterId && b.ChatId == request.ChatId, cancellationToken);

		if (banUserByChat != null) throw new ForbiddenException("You are banned");
		
		var message = await _context.Messages
			.AsNoTracking()
			.Where(m => m.Id == request.MessageId && m.ChatId == request.ChatId)
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
			.FirstOrDefaultAsync(m => m.Id == request.MessageId, cancellationToken);
		
		if (message == null) throw new DbEntityNotFoundException("Message not found");

		return message;
	}
}