using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Hubs;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.BusinessLogic.Services;
using Messenger.Domain.Entities;
using Messenger.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiCommands.Messages;

public class DeleteMessageCommandHandler : IRequestHandler<DeleteMessageCommand, Result<MessageDto>>
{
	private readonly IHubContext<ChatHub, IChatHub> _hubContext;
	private readonly DatabaseContext _context;
	private readonly IFileService _fileService;

	public DeleteMessageCommandHandler(DatabaseContext context, IFileService fileService, IHubContext<ChatHub, IChatHub> hubContext)
	{
		_context = context;
		_fileService = fileService;
		_hubContext = hubContext;
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

		if (message.OwnerId == request.RequesterId || message.Chat.OwnerId == request.RequesterId)
		{
			if (request.IsDeleteForAll)
			{
				foreach (var attachment in message.Attachments)
				{
					_fileService.DeleteFile(Path.Combine(BaseDirService.GetPathWwwRoot(), attachment.Link.Split("/")[^1]));
				}
			
				_context.Messages.Remove(message);
				await _context.SaveChangesAsync(cancellationToken);
			
				return new Result<MessageDto>(
					new MessageDto
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
					});
			}
			
			var deletedMessageByUser = new DeletedMessageByUser
			{
				MessageId = message.Id,
				UserId = request.RequesterId
			};

			var lastMessageNow = await _context.Messages
				.Include(m => m.Owner)
				.Where(m => m.ChatId == message.ChatId && m.Id != message.Id)
				.OrderBy(m => m.DateOfCreate)
				.LastAsync(cancellationToken);
			
			_context.DeletedMessageByUsers.Add(deletedMessageByUser);
			await _context.SaveChangesAsync(cancellationToken);

			var messageDeleteNotification = new MessageDeleteNotificationDto()
			{
				MessageId = message.Id,
				NewLastMessageId = lastMessageNow.Id,
				NewLastMessageText = lastMessageNow.Text,
				NewLastMessageAuthorDisplayName = lastMessageNow.Owner?.DisplayName,
				NewLastMessageDateOfCreate = lastMessageNow.DateOfCreate
			};
			
			await _hubContext.Clients.Group(message.ChatId.ToString()).DeleteMessageAsync(messageDeleteNotification);
			
			return new Result<MessageDto>(
				new MessageDto
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
				}); 
		}
		
		return new Result<MessageDto>(new ForbiddenError("It is forbidden to delete someone else's message"));
	}
}