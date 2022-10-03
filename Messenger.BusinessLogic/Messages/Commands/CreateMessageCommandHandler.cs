using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Exceptions;
using Messenger.BusinessLogic.Models;
using Messenger.Domain.Entities;
using Messenger.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.Messages.Commands;

public class CreateMessageCommandHandler : IRequestHandler<CreateMessageCommand, MessageDto>
{
	private readonly DatabaseContext _context;
	private readonly IFileService _fileService;
	private readonly IWebHostEnvironment _webHostEnvironment;

	public CreateMessageCommandHandler(DatabaseContext context, IFileService fileService, IWebHostEnvironment webHostEnvironment)
	{
		_context = context;
		_fileService = fileService;
		_webHostEnvironment = webHostEnvironment;
	}
	
	public async Task<MessageDto> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
	{
		var chatUser = await _context.ChatUsers
			.FirstOrDefaultAsync(c => c.UserId == request.RequestorId && c.ChatId == request.ChatId, cancellationToken);

		var banUserByChat = await _context.BanUserByChats
			.FirstOrDefaultAsync(b => b.UserId == request.RequestorId && b.ChatId == request.ChatId, cancellationToken);
		
		if (chatUser?.MuteDateOfExpire < DateTime.UtcNow)
		{
			chatUser.MuteDateOfExpire = null;
		}
		
		if (banUserByChat?.BanDateOfExpire < DateTime.UtcNow)
		{
			_context.BanUserByChats.Remove(banUserByChat);
		}
		
		if (chatUser is { MuteDateOfExpire: null } &&
		     (banUserByChat == null ||
		     banUserByChat.BanDateOfExpire < DateTime.UtcNow) ||
		    chatUser?.Chat.OwnerId == request.RequestorId)
		{
			if (request.Files?.Count > 4)
				throw new ForbiddenException("You cannot send more than 4 files");
			
			var newMessage = new Message(
				text: request.Text,
				ownerId: request.RequestorId,
				replyToMessageId: request.ReplyToId,
				chatId: request.ChatId);

			if (request.Files != null)
			{
				var attachments = new List<Attachment>();
			
				foreach (var file in request.Files)
				{
					var fileLink = await _fileService.CreateFileAsync(_webHostEnvironment.WebRootPath, file);

					var attachment = new Attachment(
						name: file.FileName,
						size: file.Length,
						messageId: newMessage.Id,
						link: fileLink);
				
					attachments.Add(attachment);
				}
			
				newMessage.Attachments.AddRange(attachments);
			}

			chatUser.Chat.LastMessageId = newMessage.Id;
		
			_context.Messages.Add(newMessage);
			_context.ChatUsers.Update(chatUser);
			await _context.SaveChangesAsync(cancellationToken);

			return new MessageDto
			{
				Id = newMessage.Id,
				Text = newMessage.Text,
				IsEdit = false,
				OwnerId = newMessage.OwnerId,
				OwnerDisplayName = newMessage.Owner?.DisplayName,
				OwnerAvatarLink = newMessage.Owner?.AvatarLink,
				ReplyToMessageId = newMessage.ReplyToMessageId,
				ReplyToMessageText = newMessage.ReplyToMessage?.Text,
				ReplyToMessageAuthorDisplayName = newMessage.ReplyToMessage?.Owner?.DisplayName,
				ChatId = newMessage.ChatId,
				DateOfCreate = newMessage.DateOfCreate,
			};
		}
		
		throw new ForbiddenException("It is forbidden to send messages to the chat");
	}
}