using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.BusinessLogic.Services;
using Messenger.Domain.Constants;
using Messenger.Domain.Entities;
using Messenger.Services;

namespace Messenger.BusinessLogic.ApiCommands.Messages;

public class DeleteMessageCommandHandler : IRequestHandler<DeleteMessageCommand, Result<MessageDto>>
{
	private readonly DatabaseContext _context;
	private readonly IFileService _fileService;

	public DeleteMessageCommandHandler(DatabaseContext context, IFileService fileService)
	{
		_context = context;
		_fileService = fileService;
	}
	
	public async Task<Result<MessageDto>> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
	{
		var message = await _context.Messages.FindAsync(request.MessageId, cancellationToken);
		
		if (message == null) return new Result<MessageDto>(new DbEntityNotFoundError("Message not found"));

		if (message.OwnerId == request.RequesterId || message.Chat.Owner?.Id == request.RequesterId)
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
			
			_context.DeletedMessageByUsers.Add(deletedMessageByUser);
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
		
		return new Result<MessageDto>(new ForbiddenError("It is forbidden to delete someone else's message"));
	}
}