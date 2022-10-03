using MediatR;
using Messenger.BusinessLogic.Exceptions;
using Messenger.BusinessLogic.Models;
using Messenger.Domain.Entities;
using Messenger.Domain.Enum;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.Dialogs.Command;

public class CreateDialogCommandHandler : IRequestHandler<CreateDialogCommand, ChatDto>
{
	private readonly DatabaseContext _context;

	public CreateDialogCommandHandler(DatabaseContext context)
	{
		_context = context;
	}
	
	public async Task<ChatDto> Handle(CreateDialogCommand request, CancellationToken cancellationToken)
	{
		var requestor = await _context.Users.FindAsync(request.RequesterId);
		
		if (requestor == null) throw new Exception("Requestor not found");
		
		var user = await _context.Users.FindAsync(request.UserId);
		
		if (user == null) throw new DbEntityNotFoundException("User not found");

		var dialog = await (
				from chatUser1 in _context.ChatUsers
				join chatUser2 in _context.ChatUsers
					on new {x1 = chatUser1.UserId, x2 = chatUser1.ChatId} equals new {x1 = chatUser2.UserId, x2 = chatUser2.ChatId}
				where chatUser1.Chat.Type == ChatType.Dialog &&
				      chatUser1.UserId == request.RequesterId && chatUser2.UserId == request.UserId
				select chatUser2)
			.FirstOrDefaultAsync(cancellationToken);

		if (dialog != null)
			throw new DbEntityExistsException("Dialog already exists");

		var newDialog = new Chat(
			name: null,
			title: null,
			type: ChatType.Dialog,
			ownerId: null,
			avatarLink: null,
			lastMessageId: null);
		
		_context.Chats.Add(newDialog);
		_context.ChatUsers.Add(new ChatUser {ChatId = newDialog.Id, UserId = request.RequesterId});
		_context.ChatUsers.Add(new ChatUser {ChatId = newDialog.Id, UserId = request.UserId});
		await _context.SaveChangesAsync(cancellationToken);

		return new ChatDto
		{
			Id = newDialog.Id,
			Name = newDialog.Name,
			Title = newDialog.Title,
			Type = newDialog.Type,
			AvatarLink = user.AvatarLink,
			MembersCount = 2,
			IsMember = true
		};
	}
}