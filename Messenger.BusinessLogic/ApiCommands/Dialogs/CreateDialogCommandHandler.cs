using MediatR;
using Messenger.BusinessLogic.Hubs;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Entities;
using Messenger.Domain.Enum;
using Messenger.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiCommands.Dialogs;

public class CreateDialogCommandHandler : IRequestHandler<CreateDialogCommand, Result<ChatDto>>
{
	private readonly DatabaseContext _context;
	private readonly IHubContext<ChatHub, IChatHub> _hubContext;

	public CreateDialogCommandHandler(DatabaseContext context, IHubContext<ChatHub, IChatHub> hubContext)
	{
		_context = context;
		_hubContext = hubContext;
	}
	
	public async Task<Result<ChatDto>> Handle(CreateDialogCommand request, CancellationToken cancellationToken)
	{
		var user = await _context.Users
			.AsNoTracking()
			.FirstOrDefaultAsync(u => u.Id == request.UserId, CancellationToken.None);

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

		var chatUserForRequester = new ChatUser { ChatId = newDialog.Id, UserId = request.RequesterId };
		var chatUserForInterlocutor = new ChatUser { ChatId = newDialog.Id, UserId = request.UserId };
		
		_context.Chats.Add(newDialog);
		_context.ChatUsers.Add(chatUserForRequester);
		_context.ChatUsers.Add(chatUserForInterlocutor);
		
		await _context.SaveChangesAsync(cancellationToken);

		await _context.Entry(newDialog).Collection(d => d.ChatUsers).LoadAsync(cancellationToken);

		foreach (var chatUser in newDialog.ChatUsers)
		{
			await _context.Entry(chatUser).Reference(c => c.User).LoadAsync(cancellationToken);
		}
		
		var chatDto = new ChatDto
		{
			Id = newDialog.Id,
			Name = newDialog.Name,
			Title = newDialog.Title,
			Type = newDialog.Type,
			AvatarLink = user.AvatarLink,
			MembersCount = 2,
			IsMember = true,
			Members = newDialog.ChatUsers.Select(c => new UserDto(c.User)).ToList()
		};
		
		await _hubContext.Clients.User(request.UserId.ToString()).CreateDialogForInterlocutor(chatDto);
		
		return new Result<ChatDto>(chatDto);
	}
}