using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiCommands.Conversations;

public class CreatePermissionsUserInConversationCommandHandler 
	: IRequestHandler<CreatePermissionsUserInConversationCommand, Result<PermissionDto>>
{
	private readonly DatabaseContext _context;

	public CreatePermissionsUserInConversationCommandHandler(DatabaseContext context)
	{
		_context = context;
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
			return new Result<PermissionDto>(new DbEntityNotFoundError("No requestor found in chat"));

		if (chatUserByRequester.Role is { CanGivePermissionToUser: true } ||
		    chatUserByRequester.Chat.OwnerId == request.RequesterId)
		{
			var chatUserByUser = await _context.ChatUsers
				.Include(c => c.User)
				.FirstOrDefaultAsync(c => c.ChatId == request.ChatId && c.UserId == request.UserId, cancellationToken);
		
			if (chatUserByUser == null) 
				return new Result<PermissionDto>(new DbEntityNotFoundError("No user found in chat"));
			
			chatUserByUser.CanSendMedia = request.CanSendMedia;

			_context.ChatUsers.Update(chatUserByUser);
			await _context.SaveChangesAsync(cancellationToken);
				
			return new Result<PermissionDto>(new PermissionDto(chatUserByUser));
		}
		
		return new Result<PermissionDto>(new ForbiddenError("No rights to create user permissions in the chat"));
	}
}