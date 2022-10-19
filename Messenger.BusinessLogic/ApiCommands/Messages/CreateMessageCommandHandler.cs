using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.BusinessLogic.Services;
using Messenger.Domain.Constants;
using Messenger.Domain.Entities;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Messenger.BusinessLogic.ApiCommands.Messages;

public class CreateMessageCommandHandler : IRequestHandler<CreateMessageCommand, Result<MessageDto>>
{
	private readonly DatabaseContext _context;
	private readonly IFileService _fileService;
	private readonly IConfiguration _configuration;

	public CreateMessageCommandHandler(DatabaseContext context, IFileService fileService, IConfiguration configuration)
	{
		_context = context;
		_fileService = fileService;
		_configuration = configuration;
	}
	
	public async Task<Result<MessageDto>> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
	{
		var chatUser = await _context.ChatUsers
			.Include(c => c.Chat)
			.FirstOrDefaultAsync(c => c.UserId == request.RequesterId && c.ChatId == request.ChatId, cancellationToken);

		var banUserByChat = await _context.BanUserByChats
			.FirstOrDefaultAsync(b => b.UserId == request.RequesterId && b.ChatId == request.ChatId, cancellationToken);
		
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
		    chatUser?.Chat.OwnerId == request.RequesterId)
		{
			if (request.Files?.Count > 4)
				return new Result<MessageDto>(new ForbiddenError("You cannot send more than 4 files"));
			
			var newMessage = new Message(
				text: request.Text,
				ownerId: request.RequesterId,
				replyToMessageId: request.ReplyToId,
				chatId: request.ChatId);

			if (request.Files != null)
			{
				var attachments = new List<Attachment>();
			
				foreach (var file in request.Files)
				{
					var fileLink = await _fileService.CreateFileAsync(BaseDirService.GetPathWwwRoot(), file,
						_configuration[AppSettingConstants.MessengerDomainName]);

					var attachment = new Attachment(
						name: file.FileName,
						size: file.Length,
						messageId: newMessage.Id,
						link: fileLink);
				
					attachments.Add(attachment);
				}
			
				newMessage.Attachments.AddRange(attachments);
			}

		
			_context.Messages.Add(newMessage);
			
			chatUser.Chat.LastMessageId = newMessage.Id;
			
			_context.Chats.Update(chatUser.Chat);
			await _context.SaveChangesAsync(cancellationToken);

			await _context.Entry(newMessage).Reference(m => m.Owner).LoadAsync(cancellationToken);
			await _context.Entry(newMessage).Reference(m => m.ReplyToMessage).LoadAsync(cancellationToken);

			if (newMessage.ReplyToMessage != null)
			{
				await _context.Entry(newMessage.ReplyToMessage).Reference(r => r.Owner).LoadAsync(cancellationToken);
			}

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
					Attachments = newMessage.Attachments.Select(a => new AttachmentDto(a)).ToList()
				});
		}

		return new Result<MessageDto>(new ForbiddenError("It is forbidden to send messages to the chat"));
	}
}