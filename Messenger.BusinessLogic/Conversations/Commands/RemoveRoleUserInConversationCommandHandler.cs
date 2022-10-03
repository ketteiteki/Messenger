using MediatR;
using Messenger.BusinessLogic.Exceptions;
using Messenger.BusinessLogic.Models;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.Conversations.Commands;

public class RemoveRoleUserInConversationCommandHandler 
	: IRequestHandler<RemoveRoleUserInConversationCommand, RoleUserByChatDto>
{
	private readonly DatabaseContext _context;

	public RemoveRoleUserInConversationCommandHandler(DatabaseContext context)
	{
		_context = context;
	}
 	
	public async Task<RoleUserByChatDto> Handle(RemoveRoleUserInConversationCommand request, CancellationToken cancellationToken)
	{
		var chatUser = await _context.ChatUsers
			.FirstOrDefaultAsync(r => r.UserId == request.UserId && r.ChatId == request.ChatId, cancellationToken);

		if (chatUser == null)
			throw new DbEntityNotFoundException("No requester in the chat");

		if (chatUser.Role == null)
			throw new DbEntityNotFoundException("Role not found");
			
		if (chatUser.Chat.OwnerId == request.RequesterId)
		{
			_context.RoleUserByChats.Remove(chatUser.Role);
			await _context.SaveChangesAsync(cancellationToken);
		
			return new RoleUserByChatDto(chatUser.Role);
		}
		
		throw new ForbiddenException("Only the creator can be given a role");
	}
}