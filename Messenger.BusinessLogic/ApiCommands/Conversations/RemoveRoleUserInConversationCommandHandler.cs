using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiCommands.Conversations;

public class RemoveRoleUserInConversationCommandHandler 
	: IRequestHandler<RemoveRoleUserInConversationCommand, Result<RoleUserByChatDto>>
{
	private readonly DatabaseContext _context;

	public RemoveRoleUserInConversationCommandHandler(DatabaseContext context)
	{
		_context = context;
	}
 	
	public async Task<Result<RoleUserByChatDto>> Handle(RemoveRoleUserInConversationCommand request,
		CancellationToken cancellationToken)
	{
		var chatUser = await _context.ChatUsers
			.Include(c => c.Chat)
			.Include(c => c.User)
			.Include(c => c.Role)
			.FirstOrDefaultAsync(r => r.UserId == request.UserId && r.ChatId == request.ChatId, cancellationToken);

		if (chatUser == null)
		{
			return new Result<RoleUserByChatDto>(new DbEntityNotFoundError("No requester in the chat"));
		}

		if (chatUser.Role == null)
		{
			return new Result<RoleUserByChatDto>(new DbEntityNotFoundError("Role not found"));
		}

		if (chatUser.Chat.OwnerId != request.RequesterId)
		{
			return new Result<RoleUserByChatDto>(new ForbiddenError("Only the creator can be given a role"));
		}
		
		var role = chatUser.Role;
			
		_context.RoleUserByChats.Remove(chatUser.Role);
		await _context.SaveChangesAsync(cancellationToken);
		
		return new Result<RoleUserByChatDto>(new RoleUserByChatDto(role));

	}
}