using MediatR;
using Messenger.Application.Interfaces;
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
	private readonly IBlobServiceSettings _blobServiceSettings;

	public CreateDialogCommandHandler(
		DatabaseContext context,
		IHubContext<ChatHub, IChatHub> hubContext,
		IBlobServiceSettings blobServiceSettings)
	{
		_context = context;
		_hubContext = hubContext;
		_blobServiceSettings = blobServiceSettings;
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

		var newDialog = new ChatEntity(
			name: null,
			title: null,
			ChatType.Dialog,
			ownerId: null,
			avatarFileName: null,
			lastMessageId: null);

		var chatUserForRequester = new ChatUserEntity(
			request.RequesterId, 
			newDialog.Id, 
			canSendMedia: true, 
			muteDateOfExpire: null);
		
		var chatUserForInterlocutor = new ChatUserEntity(
			request.UserId,
			newDialog.Id,
			canSendMedia: true,
			muteDateOfExpire: null);
		
		_context.Chats.Add(newDialog);
		_context.ChatUsers.Add(chatUserForRequester);
		_context.ChatUsers.Add(chatUserForInterlocutor);
		
		await _context.SaveChangesAsync(cancellationToken);

		await _context.Entry(newDialog).Collection(d => d.ChatUsers).LoadAsync(cancellationToken);

		foreach (var chatUser in newDialog.ChatUsers)
		{
			await _context.Entry(chatUser).Reference(c => c.User).LoadAsync(cancellationToken);
		}

		var requester = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.RequesterId, cancellationToken);

		var avatarLink = requester.AvatarFileName != null
			? $"{_blobServiceSettings.MessengerBlobAccess}/{requester.AvatarFileName}"
			: null;
		
		var chatDto = new ChatDto
		{
			Id = newDialog.Id,
			Name = newDialog.Name,
			Title = newDialog.Title,
			Type = newDialog.Type,
			AvatarLink = avatarLink,
			MembersCount = 2,
			IsMember = true,
			Members = newDialog.ChatUsers
				.Select(c => new UserDto(
					c.User.Id,
					c.User.DisplayName,
					c.User.Nickname,
					c.User.Bio,
					c.User.AvatarFileName != null ? 
						$"{_blobServiceSettings.MessengerBlobAccess}/{c.User.AvatarFileName}" 
						: null))
				.ToList()
		};
		
		await _hubContext.Clients.User(request.UserId.ToString()).CreateDialogForInterlocutor(chatDto);
		
		return new Result<ChatDto>(chatDto);
	}
}