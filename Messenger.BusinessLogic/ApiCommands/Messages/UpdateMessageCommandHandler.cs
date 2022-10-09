using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Services;

namespace Messenger.BusinessLogic.ApiCommands.Messages;

public class UpdateMessageCommandHandler : IRequestHandler<UpdateMessageCommand, Result<MessageDto>>
{
	private readonly DatabaseContext _context;

	public UpdateMessageCommandHandler(DatabaseContext context)
	{
		_context = context;
	}
	
	public async Task<Result<MessageDto>> Handle(UpdateMessageCommand request, CancellationToken cancellationToken)
	{
		var message = await _context.Messages.FindAsync(request.MessageId);
		if (message == null) return new Result<MessageDto>(new DbEntityNotFoundError("Message not found")); 

		if (request.RequestorId != message.OwnerId)
			return new Result<MessageDto>(new ForbiddenError("It is forbidden to change someone else's message")); 
		
		message.Text = request.Text;
		message.IsEdit = true;
		
		_context.Messages.Update(message);
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
}