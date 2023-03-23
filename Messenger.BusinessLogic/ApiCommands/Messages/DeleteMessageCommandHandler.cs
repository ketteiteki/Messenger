using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Hubs;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Entities;
using Messenger.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiCommands.Messages;

public class DeleteMessageCommandHandler : IRequestHandler<DeleteMessageCommand, Result<MessageDto>>
{
	private readonly IHubContext<ChatHub, IChatHub> _hubContext;
	private readonly DatabaseContext _context;
	private readonly IBlobService _blobService;
	private readonly IBlobServiceSettings _blobServiceServiceSettings;

	public DeleteMessageCommandHandler(
		DatabaseContext context, 
		IHubContext<ChatHub, IChatHub> hubContext,
		IBlobService blobService, 
		IBlobServiceSettings blobServiceServiceSettings)
	{
		_context = context;
		_hubContext = hubContext;
		_blobService = blobService;
		_blobServiceServiceSettings = blobServiceServiceSettings;
	}
	
	public async Task<Result<MessageDto>> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
	{
		var message = await _context.Messages
			.Include(m => m.Chat)
			.Include(m => m.Owner)
			.Include(m => m.Attachments)
			.FirstOrDefaultAsync(m => m.Id == request.MessageId, cancellationToken);

		if (message == null)
		{
			return new Result<MessageDto>(new DbEntityNotFoundError("Message not found"));
		}

		if (message.OwnerId != request.RequesterId && message.Chat.OwnerId != request.RequesterId)
		{
			return new Result<MessageDto>(new ForbiddenError("It is forbidden to delete someone else's message"));
		}		

		if (request.IsDeleteForAll)
		{
			foreach (var attachment in message.Attachments)
			{
				await _blobService.DeleteBlobAsync(attachment.FileName);
			}
			
			_context.Messages.Remove(message);
			_context.Attachments.RemoveRange(message.Attachments);

			await _context.SaveChangesAsync(cancellationToken);

			var messageDtoDeleteForAll = new MessageDto
			{
				Id = message.Id,
				Text = message.Text,
				IsEdit = message.IsEdit,
				OwnerId = message.OwnerId,
				OwnerDisplayName = message.Owner?.DisplayName,
				OwnerAvatarLink = message.Owner?.AvatarFileName != null ? 
					$"{_blobServiceServiceSettings.MessengerBlobAccess}/{message.Owner.AvatarFileName}" 
					: null,
				ReplyToMessageId = message.ReplyToMessageId,
				ReplyToMessageText = message.ReplyToMessage?.Text,
				ReplyToMessageAuthorDisplayName = message.ReplyToMessage?.Owner?.DisplayName,
				Attachments = new List<AttachmentDto>(),
				ChatId = message.ChatId,
				DateOfCreate = message.DateOfCreate
			};
			
			return new Result<MessageDto>(messageDtoDeleteForAll);
		}

		var deletedMessageByUser = new DeletedMessageByUserEntity(message.Id, request.RequesterId);

		var lastMessageNow = await _context.Messages
			.Include(m => m.Owner)
			.Where(m => m.ChatId == message.ChatId && m.Id != message.Id)
			.OrderBy(m => m.DateOfCreate)
			.LastAsync(cancellationToken);
			
		_context.DeletedMessageByUsers.Add(deletedMessageByUser);
		await _context.SaveChangesAsync(cancellationToken);

		var messageDeleteNotification = new MessageDeleteNotificationDto()
		{
			OwnerId = message.OwnerId,
			ChatId = message.ChatId,
			MessageId = message.Id,
			NewLastMessageId = lastMessageNow.Id,
			NewLastMessageText = lastMessageNow.Text,
			NewLastMessageAuthorDisplayName = lastMessageNow.Owner?.DisplayName,
			NewLastMessageDateOfCreate = lastMessageNow.DateOfCreate
		};
			
		await _hubContext.Clients.Group(message.ChatId.ToString()).DeleteMessageAsync(messageDeleteNotification);

		var messageDto = new MessageDto
		{
			Id = message.Id,
			Text = message.Text,
			IsEdit = message.IsEdit,
			OwnerId = message.OwnerId,
			OwnerDisplayName = message.Owner?.DisplayName,
			OwnerAvatarLink = message.Owner?.AvatarFileName != null ? 
				$"{_blobServiceServiceSettings.MessengerBlobAccess}/{message.Owner.AvatarFileName}" 
				: null,
			ReplyToMessageId = message.ReplyToMessageId,
			ReplyToMessageText = message.ReplyToMessage?.Text,
			ReplyToMessageAuthorDisplayName = message.ReplyToMessage?.Owner?.DisplayName,
			Attachments = message.Attachments
				.Select(a => new AttachmentDto(
					a.Id,
					a.FileName != null ? 
						$"{_blobServiceServiceSettings.MessengerBlobAccess}/{a.FileName}" 
						: null,
					a.Size))
				.ToList(),
			ChatId = message.ChatId,
			DateOfCreate = message.DateOfCreate
		};
		
		return new Result<MessageDto>(messageDto);
	}
}