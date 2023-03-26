using MediatR;
using Messenger.BusinessLogic.Hubs;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Persistence;
using Messenger.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiCommands.Conversations;

public class CreatePermissionsUserInConversationCommandHandler 
	: IRequestHandler<CreatePermissionsUserInConversationCommand, Result<PermissionDto>>
{
	private readonly DatabaseContext _context;
	private readonly IHubContext<ChatHub, IChatHub> _hubContext;

	public CreatePermissionsUserInConversationCommandHandler(DatabaseContext context, IHubContext<ChatHub, IChatHub> hubContext)
	{
		_context = context;
		_hubContext = hubContext;
	}

	public async Task<Result<PermissionDto>> Handle(CreatePermissionsUserInConversationCommand request
		, CancellationToken cancellationToken)
	{
		var chatUserByRequester = await _context.ChatUsers
			.AsNoTracking()
			.Include(c => c.Chat)
			.Include(c => c.Role)
			.FirstOrDefaultAsync(c => c.ChatId == request.ChatId && c.UserId == request.RequesterId, cancellationToken);

		if (chatUserByRequester == null)
		{
			return new Result<PermissionDto>(new ForbiddenError("No requester found in chat"));
		}

		if ((chatUserByRequester.Role == null &&
		     chatUserByRequester.Chat.OwnerId != request.RequesterId)  || 
		    (chatUserByRequester.Role is { CanGivePermissionToUser: false } &&
		     chatUserByRequester.Chat.OwnerId != request.RequesterId))
		{
			return new Result<PermissionDto>(new ForbiddenError("No rights to create user permissions in the chat"));
		}
		
		var chatUserByUser = await _context.ChatUsers
			.Include(c => c.User)
			.FirstOrDefaultAsync(c => c.ChatId == request.ChatId && c.UserId == request.UserId, cancellationToken);

		if (chatUserByUser == null)
		{
			return new Result<PermissionDto>(new DbEntityNotFoundError("No user found in chat"));
		}

		var notifyPermissionForUserDto = new NotifyPermissionForUserDto(
			request.ChatId,
			request.CanSendMedia,
			muteDateOfExpire: null); 
			
		if (request.MuteMinutes is >= 1)
		{
			var muteDateOfExpire = DateTime.UtcNow.AddMinutes((int) request.MuteMinutes);

			chatUserByUser.UpdateMuteDateOfExpire(muteDateOfExpire);	
			
			notifyPermissionForUserDto.MuteDateOfExpire = muteDateOfExpire;
		}
			
		chatUserByUser.UpdateCanSendMedia(request.CanSendMedia);
		
		_context.ChatUsers.Update(chatUserByUser);
		
		await _context.SaveChangesAsync(cancellationToken);

		await _hubContext.Clients.User(request.UserId.ToString()).NotifyPermissionForUser(notifyPermissionForUserDto);
			
		return new Result<PermissionDto>(new PermissionDto(chatUserByUser));
	}
}