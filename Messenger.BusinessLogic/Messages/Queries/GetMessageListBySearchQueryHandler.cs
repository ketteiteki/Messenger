using System.Text.RegularExpressions;
using MediatR;
using Messenger.BusinessLogic.Exceptions;
using Messenger.BusinessLogic.Models;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.Messages.Queries;

public class GetMessageListBySearchQueryHandler : IRequestHandler<GetMessageListBySearchQuery, List<MessageDto>>
{
	private readonly DatabaseContext _context;

	public GetMessageListBySearchQueryHandler(DatabaseContext context)
	{
		_context = context;
	}
	
	public async Task<List<MessageDto>> Handle(GetMessageListBySearchQuery request, CancellationToken cancellationToken)
	{
		var banUserByChat = await _context.BanUserByChats
			.FirstOrDefaultAsync(b => b.UserId == request.RequesterId && b.ChatId == request.ChatId, cancellationToken);

		if (banUserByChat != null) throw new ForbiddenException("You are banned");
		
		if (request.FromMessageDateOfCreate != null)
		{
			if (request.FromUserId != null)
			{
				var messagesFromMessageIdAmdUserId = await _context.Messages
					.AsNoTracking()
					.Where(m => m.ChatId == request.ChatId 
					            && m.OwnerId == request.FromUserId 
					            && m.DateOfCreate < request.FromMessageDateOfCreate 
					            && Regex.IsMatch(m.Text, $"{request.SearchText}", RegexOptions.IgnoreCase))
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
			
				return messagesFromMessageIdAmdUserId;
			}
			
			var messagesFromMessageId = await _context.Messages
				.AsNoTracking()
				.Where(m => m.ChatId == request.ChatId 
				            && m.DateOfCreate < request.FromMessageDateOfCreate 
				            && Regex.IsMatch(m.Text, $"{request.SearchText}", RegexOptions.IgnoreCase))
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
			
			return messagesFromMessageId;
		}
		
		if (request.FromUserId != null)
		{
			var messagesFromUserId = await _context.Messages
				.AsNoTracking()
				.Where(m => m.ChatId == request.ChatId 
				            && m.OwnerId == request.FromUserId 
				            && Regex.IsMatch(m.Text, $"{request.SearchText}", RegexOptions.IgnoreCase))
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
			
			return messagesFromUserId;
		}
		
		var messages = await _context.Messages
			.AsNoTracking()
			.Where(m => m.ChatId == request.ChatId 
			            && Regex.IsMatch(m.Text, $"{request.SearchText}", RegexOptions.IgnoreCase))
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
			
		return messages;
	}
}