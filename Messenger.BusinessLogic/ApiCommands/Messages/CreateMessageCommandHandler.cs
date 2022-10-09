using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Constants;
using Messenger.Domain.Entities;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiCommands.Messages;

public class CreateMessageCommandHandler : IRequestHandler<CreateMessageCommand, Result<MessageDto>>
{
	private readonly DatabaseContext _context;
	private readonly IFileService _fileService;

	public CreateMessageCommandHandler(DatabaseContext context, IFileService fileService)
	{
		_context = context;
		_fileService = fileService;
	}
	
	public async Task<Result<MessageDto>> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
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
				return new Result<MessageDto>(new ForbiddenError("You cannot send more than 4 files"));
			
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
					var fileLink = await _fileService.CreateFileAsync(EnvironmentConstants.GetPathWWWRoot(), file);

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

			return new Result<MessageDto>(
				new MessageDto
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
				});
		}

		return new Result<MessageDto>(new ForbiddenError("It is forbidden to send messages to the chat"));
	}
}