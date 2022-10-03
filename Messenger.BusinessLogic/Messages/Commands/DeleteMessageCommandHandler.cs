using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Exceptions;
using Messenger.BusinessLogic.Models;
using Messenger.Domain.Entities;
using Messenger.Services;
using Microsoft.AspNetCore.Hosting;

namespace Messenger.BusinessLogic.Messages.Commands;

public class DeleteMessageCommandHandler : IRequestHandler<DeleteMessageCommand, MessageDto>
{
	private readonly DatabaseContext _context;
	private readonly IWebHostEnvironment _webHostEnvironment;
	private readonly IFileService _fileService;

	public DeleteMessageCommandHandler(DatabaseContext context, IFileService fileService, IWebHostEnvironment webHostEnvironment)
	{
		_context = context;
		_fileService = fileService;
		_webHostEnvironment = webHostEnvironment;
	}
	
	public async Task<MessageDto> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
	{
		var message = await _context.Messages.FindAsync(request.MessageId, cancellationToken);
		
		if (message == null) throw new DbEntityNotFoundException("Message not found");

		if (message.OwnerId == request.RequesterId || message.Chat.Owner?.Id == request.RequesterId)
		{
			if (request.IsDeleteForAll)
			{
				foreach (var attachment in message.Attachments)
				{
					_fileService.DeleteFile(Path.Combine(_webHostEnvironment.WebRootPath, attachment.Link.Split("/")[^1]));
				}
			
				_context.Messages.Remove(message);
				await _context.SaveChangesAsync(cancellationToken);
			
				return new MessageDto
				{
					Id = message.Id,
					Text = message.Text,
					IsEdit = message.IsEdit,
					OwnerId = message.OwnerId,
					OwnerDisplayName = message.Owner?.DisplayName,
					OwnerAvatarLink = message.Owner?.AvatarLink,
					ReplyToMessageId = message.ReplyToMessageId,
					ReplyToMessageText = message.ReplyToMessage?.Text,
					ReplyToMessageAuthorDisplayName = message.ReplyToMessage?.Owner?.DisplayName,
					Attachments = message.Attachments.Select(a => new AttachmentDto(a)).ToList(),
					ChatId = message.ChatId,
					DateOfCreate = message.DateOfCreate
				};
			}
			
			var deletedMessageByUser = new DeletedMessageByUser
			{
				MessageId = message.Id,
				UserId = request.RequesterId
			};
			
			_context.DeletedMessageByUsers.Add(deletedMessageByUser);
			await _context.SaveChangesAsync(cancellationToken);
			
			return new MessageDto
			{
				Id = message.Id,
				Text = message.Text,
				IsEdit = message.IsEdit,
				OwnerId = message.OwnerId,
				OwnerDisplayName = message.Owner?.DisplayName,
				OwnerAvatarLink = message.Owner?.AvatarLink,
				ReplyToMessageId = message.ReplyToMessageId,
				ReplyToMessageText = message.ReplyToMessage?.Text,
				ReplyToMessageAuthorDisplayName = message.ReplyToMessage?.Owner?.DisplayName,
				Attachments = message.Attachments.Select(a => new AttachmentDto(a)).ToList(),
				ChatId = message.ChatId,
				DateOfCreate = message.DateOfCreate
			}; 
		}
		
		throw new ForbiddenException("It is forbidden to delete someone else's message");
	}
}