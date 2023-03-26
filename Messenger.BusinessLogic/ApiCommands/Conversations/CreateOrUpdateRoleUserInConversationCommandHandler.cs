using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Entities;
using Messenger.Persistence;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiCommands.Conversations;

public class CreateOrUpdateRoleUserInConversationCommandHandler 
	: IRequestHandler<CreateOrUpdateRoleUserInConversationCommand, Result<RoleUserByChatDto>>
{
	private readonly DatabaseContext _context;

	public CreateOrUpdateRoleUserInConversationCommandHandler(DatabaseContext context)
	{
		_context = context;
	}
	
	public async Task<Result<RoleUserByChatDto>> Handle(CreateOrUpdateRoleUserInConversationCommand request,
		CancellationToken cancellationToken)
	{
		var chatUser = await _context.ChatUsers
			.Include(c => c.User)
			.Include(c => c.Chat)
			.Include(c => c.Role)
			.FirstOrDefaultAsync(c => c.UserId == request.UserId && c.ChatId == request.ChatId, cancellationToken);

		if (chatUser == null)
		{
			return new Result<RoleUserByChatDto>(new DbEntityNotFoundError("No user found in chat"));
		}

		if (chatUser.Chat.OwnerId != request.RequesterId)
		{
			return new Result<RoleUserByChatDto>(new ForbiddenError("Only the creator can be given a role"));
		}
		
		if (chatUser.Role == null)
		{
			var isOwner = chatUser.Chat.OwnerId == request.UserId;
			
			var role = new RoleUserByChatEntity(
				chatUser.User.Id,
				chatUser.Chat.Id,
				request.RoleTitle,
				request.RoleColor,
				request.CanBanUser,
				request.CanChangeChatData,
				request.CanGivePermissionToUser,
				request.CanAddAndRemoveUserToConversation,
				isOwner);

			_context.RoleUserByChats.Add(role);
			
			await _context.SaveChangesAsync(cancellationToken);
		
			return new Result<RoleUserByChatDto>(new RoleUserByChatDto(role));
		}

		chatUser.Role.UpdateRole(
			request.RoleTitle,
			request.RoleColor,
			request.CanBanUser,
			request.CanChangeChatData,
			request.CanAddAndRemoveUserToConversation,
			request.CanGivePermissionToUser);
			
		_context.ChatUsers.Update(chatUser);
		
		await _context.SaveChangesAsync(cancellationToken);
		
		return new Result<RoleUserByChatDto>(new RoleUserByChatDto(chatUser.Role));

	}
}