using MediatR;
using Messenger.BusinessLogic.Exceptions;
using Messenger.BusinessLogic.Models;
using Messenger.Services;

namespace Messenger.BusinessLogic.Messages.Commands;

public class UpdateMessageCommandHandler : IRequestHandler<UpdateMessageCommand, MessageDto>
{
	private readonly DatabaseContext _context;

	public UpdateMessageCommandHandler(DatabaseContext context)
	{
		_context = context;
	}
	
	public async Task<MessageDto> Handle(UpdateMessageCommand request, CancellationToken cancellationToken)
	{
		var message = await _context.Messages.FindAsync(request.MessageId);
		if (message == null) throw new DbEntityNotFoundException("Message not found");

		if (request.RequesterId != message.OwnerId)
			throw new ForbiddenException("It is forbidden to change someone else's message");
		
		message.Text = request.Text;
		message.IsEdit = true;
		
		_context.Messages.Update(message);
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
}