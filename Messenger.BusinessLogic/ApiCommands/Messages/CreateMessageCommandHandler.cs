using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Hubs;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Entities;
using Messenger.Domain.Enums;
using Messenger.Persistence;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiCommands.Messages;

public class CreateMessageCommandHandler : IRequestHandler<CreateMessageCommand, Result<MessageDto>>
{
	private readonly IHubContext<ChatHub, IChatHub> _hubContext;
	private readonly DatabaseContext _context;
	private readonly IBlobService _blobService;
	private readonly IBlobServiceSettings _blobServiceSettings;

	public CreateMessageCommandHandler(
		DatabaseContext context,
		IHubContext<ChatHub, IChatHub> hubContext, 
		IBlobService blobService,
		IBlobServiceSettings blobServiceSettings)
	{
		_context = context;
		_hubContext = hubContext;
		_blobService = blobService;
		_blobServiceSettings = blobServiceSettings;
	}
	
	public async Task<Result<MessageDto>> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
	{
		var chatUser = await _context.ChatUsers
			.Include(c => c.Chat)
			.FirstOrDefaultAsync(c => c.UserId == request.RequesterId && c.ChatId == request.ChatId, cancellationToken);

		if (chatUser == null)
		{
			return new Result<MessageDto>(new ForbiddenError("You're not in the chat"));
		}

		var isRequesterOwner = chatUser.Chat.OwnerId == request.RequesterId;

		if (chatUser.Chat.Type == ChatType.Channel && !isRequesterOwner)
		{
			return new Result<MessageDto>(new ForbiddenError("Only the owner can send messages to the channel"));
		}
		
		var banUserByChat = await _context.BanUserByChats
			.FirstOrDefaultAsync(b => b.UserId == request.RequesterId && b.ChatId == request.ChatId, cancellationToken);

		var isReplyToMessageExists = await _context.Messages
			.AnyAsync(m => m.Id == request.ReplyToId && m.ChatId == request.ChatId, cancellationToken);
		
		if (request.ReplyToId != null && !isReplyToMessageExists)
		{
			return new Result<MessageDto>(new DbEntityNotFoundError("Reply To Message not found in this chat"));
		} 
		
		var isRequesterMuted = chatUser.MuteDateOfExpire > DateTime.UtcNow;
		var isRequesterBanned = banUserByChat != null && banUserByChat.BanDateOfExpire > DateTime.UtcNow;
		
		if ((isRequesterMuted || isRequesterBanned) && !isRequesterOwner)
		{
			return new Result<MessageDto>(new ForbiddenError("It is forbidden to send messages to the chat"));
		}

		if (request.Files?.Count > 4)
		{
			return new Result<MessageDto>(new ForbiddenError("You cannot send more than 4 files"));
		}

		if (!isRequesterMuted)
		{
			chatUser.UpdateMuteDateOfExpire(null);
		}
		
		if (banUserByChat != null && !isRequesterBanned)
		{
			_context.BanUserByChats.Remove(banUserByChat);
		}

		var newMessage = new MessageEntity(
				request.Text,
				request.RequesterId,
				request.ReplyToId,
				request.ChatId);
		
		if (request.Files != null)
		{
			var attachments = new List<AttachmentEntity>();
        			
			foreach (var file in request.Files)
			{
				var attachmentFileName = await _blobService.UploadFileBlobAsync(file);

				var attachment = new AttachmentEntity(
					attachmentFileName,
					file.Length,
					newMessage.Id);
				
				attachments.Add(attachment);
			}
        			
			newMessage.Attachments.AddRange(attachments);
		}
        
		_context.Messages.Add(newMessage);
        			
        chatUser.Chat.UpdateLastMessageId(newMessage.Id);			
		
		_context.Chats.Update(chatUser.Chat);
        			
		await _context.SaveChangesAsync(cancellationToken);
        
		await _context.Entry(newMessage).Reference(m => m.Owner).LoadAsync(cancellationToken);
		await _context.Entry(newMessage).Reference(m => m.ReplyToMessage).LoadAsync(cancellationToken);
        
		if (newMessage.ReplyToMessage != null)
		{
			await _context.Entry(newMessage.ReplyToMessage).Reference(r => r.Owner).LoadAsync(cancellationToken);
		}
		
		var messageDto = new MessageDto
		{
			Id = newMessage.Id,
			Text = newMessage.Text,
			IsEdit = false,
			OwnerId = newMessage.OwnerId,
			OwnerDisplayName = newMessage.Owner?.DisplayName,
			OwnerAvatarLink = newMessage.Owner?.AvatarFileName != null ? 
				$"{_blobServiceSettings.MessengerBlobAccess}/{newMessage.Owner.AvatarFileName}" 
				: null,
			ReplyToMessageId = newMessage.ReplyToMessageId,
			ReplyToMessageText = newMessage.ReplyToMessage?.Text,
			ReplyToMessageAuthorDisplayName = newMessage.ReplyToMessage?.Owner?.DisplayName,
			ChatId = newMessage.ChatId,
			DateOfCreate = newMessage.DateOfCreate,
			Attachments = newMessage.Attachments
				.Select(a => new AttachmentDto(
					a.Id, 
					$"{_blobServiceSettings.MessengerBlobAccess}/{a.FileName}", 
					a.Size))
				.ToList()
		};
        			
		await _hubContext.Clients.Group(request.ChatId.ToString()).BroadcastMessageAsync(messageDto);
        			
		return new Result<MessageDto>(messageDto);
	}
}