using System.Text.RegularExpressions;
using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiQueries.Messages;

public class GetMessageListBySearchQueryHandler : IRequestHandler<GetMessageListBySearchQuery, Result<List<MessageDto>>>
{
	private readonly DatabaseContext _context;

	public GetMessageListBySearchQueryHandler(DatabaseContext context)
	{
		_context = context;
	}
	
	public async Task<Result<List<MessageDto>>> Handle(GetMessageListBySearchQuery request, CancellationToken cancellationToken)
	{
		var banUserByChat = await _context.BanUserByChats
			.FirstOrDefaultAsync(b => b.UserId == request.RequestorId && b.ChatId == request.ChatId, cancellationToken);

		if (banUserByChat != null) return new Result<List<MessageDto>>(new ForbiddenError("You are banned"));
		
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
			
				return new Result<List<MessageDto>>(messagesFromMessageIdAmdUserId);
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
			
			return new Result<List<MessageDto>>(messagesFromMessageId);
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
			
			return new Result<List<MessageDto>>(messagesFromUserId);
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
			
		return new Result<List<MessageDto>>(messages);
	}
}