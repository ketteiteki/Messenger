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

public class DeleteDialogCommandHandler : IRequestHandler<DeleteDialogCommand, Result<ChatDto>>
{
	private readonly DatabaseContext _context;
	private readonly IHubContext<ChatHub, IChatHub> _hubContext;

	public DeleteDialogCommandHandler(DatabaseContext context, IHubContext<ChatHub, IChatHub> hubContext)
	{
		_context = context;
		_hubContext = hubContext;
	} 
	
	public async Task<Result<ChatDto>> Handle(DeleteDialogCommand request, CancellationToken cancellationToken)
	{
		var chatUser = await _context.ChatUsers
			.Include(c => c.Chat)
			.ThenInclude(c => c.Owner)
			.FirstOrDefaultAsync(c => c.UserId == request.RequesterId && c.ChatId == request.ChatId, cancellationToken);

		if (chatUser == null)
		{
			return new Result<ChatDto>(new DbEntityNotFoundError("Dialog not found"));
		}
		
		var deletedDialogByUsers = await _context.DeletedDialogByUsers
			.Where(d => d.ChatId == request.ChatId && d.Chat.Type == ChatType.Dialog)
			.ToListAsync(cancellationToken);
		
		if (request.IsDeleteForAll || deletedDialogByUsers.Count == 1)
		{
			var chatUsers = await _context.ChatUsers
				.Include(c => c.User)
				.Where(c => c.ChatId == request.ChatId)
				.ToListAsync(cancellationToken);
			
			var interlocutorGuid = chatUsers.FirstOrDefault(c => c.UserId != request.RequesterId)?.UserId;
			
			_context.DeletedDialogByUsers.RemoveRange(deletedDialogByUsers);
			_context.ChatUsers.RemoveRange(chatUsers);
			foreach (var chatUserItem in chatUsers)
			{
				_context.Chats.Remove(chatUserItem.Chat);
			}
			
			await _context.SaveChangesAsync(cancellationToken);

			if (interlocutorGuid != null)
			{
				await _hubContext.Clients.User(interlocutorGuid.ToString()).DeleteDialogForInterlocutor(request.ChatId.ToString());
			}
			
			return new Result<ChatDto>(new ChatDto
			{
				Id = chatUser.Chat.Id,
				Name = chatUser.Chat.Name,
				Title = chatUser.Chat.Title,
				Type = chatUser.Chat.Type,
				AvatarLink = null,
				LastMessageId = chatUser.Chat.LastMessageId,
				LastMessageText = chatUser.Chat.LastMessage?.Text,
				LastMessageAuthorDisplayName = chatUser.Chat.LastMessage != null &&
				                               chatUser.Chat.LastMessage.Owner != null
					? chatUser.Chat.LastMessage.Owner.DisplayName
					: null,
				LastMessageDateOfCreate = chatUser.Chat.LastMessage?.DateOfCreate,
				MembersCount = 2,
				CanSendMedia = false,
				IsMember = false
			});
		}

		var deleteDialogByUser = new DeletedDialogByUser
		{
			ChatId = request.ChatId,
			UserId = request.RequesterId
		};

		_context.DeletedDialogByUsers.Add(deleteDialogByUser);
		await _context.SaveChangesAsync(cancellationToken);
		
		return new Result<ChatDto>(
			new ChatDto
			{
				Id = chatUser.Chat.Id,
				Name = chatUser.Chat.Name,
				Title = chatUser.Chat.Title,
				Type = chatUser.Chat.Type,
				AvatarLink = null,
				LastMessageId = chatUser.Chat.LastMessageId,
				LastMessageText = chatUser.Chat.LastMessage?.Text,
				LastMessageAuthorDisplayName = chatUser.Chat.LastMessage != null && 
				                               chatUser.Chat.LastMessage.Owner != null ?
					chatUser.Chat.LastMessage.Owner.DisplayName : null,
				LastMessageDateOfCreate = chatUser.Chat.LastMessage?.DateOfCreate,
				MembersCount = 2,
				CanSendMedia = false,
				IsMember = false
			});
	}
}