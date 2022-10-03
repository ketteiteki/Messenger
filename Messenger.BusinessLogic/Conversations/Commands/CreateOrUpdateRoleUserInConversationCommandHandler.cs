using MediatR;
using Messenger.BusinessLogic.Exceptions;
using Messenger.BusinessLogic.Models;
using Messenger.Domain.Entities;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.Conversations.Commands;

public class CreateOrUpdateRoleUserInConversationCommandHandler 
	: IRequestHandler<CreateOrUpdateRoleUserInConversationCommand, RoleUserByChatDto>
{
	private readonly DatabaseContext _context;

	public CreateOrUpdateRoleUserInConversationCommandHandler(DatabaseContext context)
	{
		_context = context;
	}
	
	public async Task<RoleUserByChatDto> Handle(CreateOrUpdateRoleUserInConversationCommand request, CancellationToken cancellationToken)
	{
		var chatUser = await _context.ChatUsers
			.FirstOrDefaultAsync(c => c.UserId == request.UserId && c.ChatId == request.ChatId, cancellationToken);

		if (chatUser == null)
			throw new ForbiddenException("No user found in chat");
		
		if (chatUser.Chat.OwnerId == request.RequesterId)
		{
			if (chatUser.Role == null)
			{
				var role = new RoleUserByChat(
					userId: chatUser.User.Id,
					chatId: chatUser.Chat.Id,
					roleTitle: request.RoleTitle,
					roleColor: request.RoleColor,
					canBanUser: request.CanBanUser,
					canChangeChatData: request.CanChangeChatData,
					canAddAndRemoveUserToConversation: request.CanAddAndRemoveUserToConversation,
					canGivePermissionToUser: request.CanGivePermissionToUser,
					isOwner: chatUser.Chat.OwnerId == request.RequesterId);

				_context.RoleUserByChats.Add(role);
				await _context.SaveChangesAsync(cancellationToken);
		
				return new RoleUserByChatDto(role);
			}

			chatUser.Role.RoleTitle = request.RoleTitle;
			chatUser.Role.RoleColor = request.RoleColor;
			chatUser.Role.CanBanUser = request.CanBanUser;
			chatUser.Role.CanChangeChatData = request.CanChangeChatData;
			chatUser.Role.CanAddAndRemoveUserToConversation = request.CanAddAndRemoveUserToConversation;
			chatUser.Role.CanGivePermissionToUser = request.CanGivePermissionToUser;
			
			_context.ChatUsers.Update(chatUser);
			await _context.SaveChangesAsync(cancellationToken);
		
			return new RoleUserByChatDto(chatUser.Role);
		}
		
		throw new ForbiddenException("Only the creator can be given a role");
	}
}