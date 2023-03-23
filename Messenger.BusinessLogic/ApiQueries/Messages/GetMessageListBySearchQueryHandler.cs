using System.Text.RegularExpressions;
using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiQueries.Messages;

public class GetMessageListBySearchQueryHandler : IRequestHandler<GetMessageListBySearchQuery, Result<List<MessageDto>>>
{
	private readonly DatabaseContext _context;
	private readonly IBlobServiceSettings _blobServiceSettings;

	public GetMessageListBySearchQueryHandler(
		DatabaseContext context,
		IBlobServiceSettings blobServiceSettings)
	{
		_context = context;
		_blobServiceSettings = blobServiceSettings;
	}
	
	public async Task<Result<List<MessageDto>>> Handle(GetMessageListBySearchQuery request, CancellationToken cancellationToken)
	{
		if (request.Limit > 60)
		{
			return new Result<List<MessageDto>>(new BadRequestError("Limit exceeded. Limit: 60"));
		}
		
		var banUserByChat = await _context.BanUserByChats
			.AnyAsync(b => b.UserId == request.RequesterId && b.ChatId == request.ChatId, cancellationToken);

		if (banUserByChat)
		{
			return new Result<List<MessageDto>>(new ForbiddenError("You are banned"));
		}
		
		if (request.FromMessageDateTime != null)
		{
			var messageListFromDate =
				await (from message in _context.Messages.AsNoTracking()
						join deletedMessageByUsers in _context.DeletedMessageByUsers.AsNoTracking()
							on new { x1 = message.Id, x2 = message.OwnerId }
							equals new { x1 = deletedMessageByUsers.MessageId, x2 = (Guid?)deletedMessageByUsers.UserId }
							into deletedMessageByUsersEnumerable
						from deletedMessageByUsersItem in deletedMessageByUsersEnumerable.DefaultIfEmpty()
						where deletedMessageByUsersItem == null
						where message.ChatId == request.ChatId
						where message.DateOfCreate < request.FromMessageDateTime 
						where Regex.IsMatch(message.Text, $"{request.SearchText}")
						select new MessageDto
						{
							Id = message.Id,
							Text = message.Text,
							IsEdit = message.IsEdit,
							OwnerId = message.OwnerId,
							OwnerDisplayName = message.Owner != null ? message.Owner.DisplayName : null,
							OwnerAvatarLink = message.Owner != null ? message.Owner.AvatarFileName != null ?
								$"{_blobServiceSettings.MessengerBlobAccess}/{message.Owner.AvatarFileName}"
								: null : null,
							ReplyToMessageId = message.ReplyToMessageId,
							ReplyToMessageText = message.ReplyToMessage != null ? message.ReplyToMessage.Text : null,
							ReplyToMessageAuthorDisplayName = message.ReplyToMessage != null && message.ReplyToMessage.Owner != null ? 
								message.ReplyToMessage.Owner.DisplayName : null,
							Attachments = message.Attachments
								.Select(a => new AttachmentDto(
									a.Id,
									$"{_blobServiceSettings.MessengerBlobAccess}/{a.FileName}",
									a.Size)).ToList(),
							ChatId = message.ChatId,
							DateOfCreate = message.DateOfCreate
						}
					)
					.ToListAsync(cancellationToken);

			return new Result<List<MessageDto>>(messageListFromDate);
		}

		var messageList =
			await (from message in _context.Messages.AsNoTracking()
					join deletedMessageByUsers in _context.DeletedMessageByUsers.AsNoTracking()
						on new { x1 = message.Id, x2 = message.OwnerId }
						equals new { x1 = deletedMessageByUsers.MessageId, x2 = (Guid?)deletedMessageByUsers.UserId }
						into deletedMessageByUsersEnumerable
					from deletedMessageByUsersItem in deletedMessageByUsersEnumerable.DefaultIfEmpty()
					where deletedMessageByUsersItem == null
					where message.ChatId == request.ChatId
					where Regex.IsMatch(message.Text, $"{request.SearchText}")
					select new MessageDto
					{
						Id = message.Id,
						Text = message.Text,
						IsEdit = message.IsEdit,
						OwnerId = message.OwnerId,
						OwnerDisplayName = message.Owner != null ? message.Owner.DisplayName : null,
						OwnerAvatarLink = message.Owner != null ? message.Owner.AvatarFileName != null ?
							$"{_blobServiceSettings.MessengerBlobAccess}/{message.Owner.AvatarFileName}"
							: null : null,
						ReplyToMessageId = message.ReplyToMessageId,
						ReplyToMessageText = message.ReplyToMessage != null ? message.ReplyToMessage.Text : null,
						ReplyToMessageAuthorDisplayName = message.ReplyToMessage != null && message.ReplyToMessage.Owner != null ? 
							message.ReplyToMessage.Owner.DisplayName : null,
						Attachments = message.Attachments.Select(a => new AttachmentDto(
							a.Id,
							$"{_blobServiceSettings.MessengerBlobAccess}/{a.FileName}",
							a.Size)).ToList(),
						ChatId = message.ChatId,
						DateOfCreate = message.DateOfCreate
					}
				)
				.Take(request.Limit)
				.ToListAsync(cancellationToken);

		return new Result<List<MessageDto>>(messageList);
	}
}