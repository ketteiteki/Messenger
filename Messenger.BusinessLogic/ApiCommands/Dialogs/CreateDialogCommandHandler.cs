using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Entities;
using Messenger.Domain.Enum;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiCommands.Dialogs;

public class CreateDialogCommandHandler : IRequestHandler<CreateDialogCommand, Result<ChatDto>>
{
	private readonly DatabaseContext _context;

	public CreateDialogCommandHandler(DatabaseContext context)
	{
		_context = context;
	}
	
	public async Task<Result<ChatDto>> Handle(CreateDialogCommand request, CancellationToken cancellationToken)
	{
		var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, CancellationToken.None);

		if (user == null)
		{
			return new Result<ChatDto>(new DbEntityNotFoundError("User not found"));
		}

		var dialog = await (
				from chat in _context.Chats
				where chat.ChatUsers.Any(c => c.UserId == request.RequesterId) &&
				      chat.ChatUsers.Any(c => c.UserId == request.UserId) &&
				      (int)chat.Type == (int)ChatType.Dialog
				      select chat)
			.FirstOrDefaultAsync(cancellationToken);

		if (dialog != null)
		{
			return new Result<ChatDto>(new DbEntityExistsError("Dialog already exists"));
		}

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

		await _context.Entry(newDialog).Collection(d => d.ChatUsers).LoadAsync(cancellationToken);

		foreach (var chatUser in newDialog.ChatUsers)
		{
			await _context.Entry(chatUser).Reference(c => c.User).LoadAsync(cancellationToken);
		}
		
		return new Result<ChatDto>(
			new ChatDto
			{
				Id = newDialog.Id,
				Name = newDialog.Name,
				Title = newDialog.Title,
				Type = newDialog.Type,
				AvatarLink = user.AvatarLink,
				MembersCount = 2,
				IsMember = true,
				Members = newDialog.ChatUsers.Select(c => new UserDto(c.User)).ToList()
			});
	}
}