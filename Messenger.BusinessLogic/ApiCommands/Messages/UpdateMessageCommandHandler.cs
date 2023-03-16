using MediatR;
using Messenger.BusinessLogic.Hubs;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiCommands.Messages;

public class UpdateMessageCommandHandler : IRequestHandler<UpdateMessageCommand, Result<MessageDto>>
{
	private readonly IHubContext<ChatHub, IChatHub> _hubContext;
	private readonly DatabaseContext _context;

	public UpdateMessageCommandHandler(DatabaseContext context, IHubContext<ChatHub, IChatHub> hubContext)
	{
		_context = context;
		_hubContext = hubContext;
	}
	
	public async Task<Result<MessageDto>> Handle(UpdateMessageCommand request, CancellationToken cancellationToken)
	{
		var message = await _context.Messages
			.Include(m => m.Chat)
			.Include(m => m.Owner)
			.Include(m => m.ReplyToMessage)
			.ThenInclude(r => r.Owner)
			.Include(m => m.Attachments)
			.FirstOrDefaultAsync(m => m.Id == request.MessageId, cancellationToken);
		
		if (message == null)
		{
			return new Result<MessageDto>(new DbEntityNotFoundError("Message not found"));
		}

		if (request.RequesterId != message.OwnerId)
		{
			return new Result<MessageDto>(new ForbiddenError("It is forbidden to change someone else's message")); 
		}
		
		message.UpdateText(request.Text);
		message.UpdateIsEdit(true);
		
		_context.Messages.Update(message);
		
		await _context.SaveChangesAsync(cancellationToken);

		var messageUpdateNotification = new MessageUpdateNotificationDto
		{
			OwnerId = message.OwnerId,
			ChatId = message.ChatId,
			MessageId = message.Id,
			UpdatedText = message.Text,
			IsLastMessage = message.Chat.LastMessageId == message.Id
		};
		
		await _hubContext.Clients.Group(message.ChatId.ToString()).UpdateMessageAsync(messageUpdateNotification);

		var messageDto = new MessageDto
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
		
		return new Result<MessageDto>(messageDto);
	}
}