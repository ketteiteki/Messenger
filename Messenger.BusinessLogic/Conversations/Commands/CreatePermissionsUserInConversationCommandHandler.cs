using MediatR;
using Messenger.BusinessLogic.Exceptions;
using Messenger.BusinessLogic.Models;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.Conversations.Commands;

public class CreatePermissionsUserInConversationCommandHandler 
	: IRequestHandler<CreatePermissionsUserInConversationCommand, UserDto>
{
	private readonly DatabaseContext _context;

	public CreatePermissionsUserInConversationCommandHandler(DatabaseContext context)
	{
		_context = context;
	}

	public async Task<UserDto> Handle(CreatePermissionsUserInConversationCommand request, CancellationToken cancellationToken)
	{
		var chatUserByRequester = await _context.ChatUsers
			.FirstOrDefaultAsync(c => c.ChatId == request.ChatId && c.UserId == request.RequesterId, cancellationToken);
		
		if (chatUserByRequester == null) throw new DbEntityNotFoundException("No requester	 found in chat");

		if (chatUserByRequester.Role is { CanGivePermissionToUser: true } ||
		    chatUserByRequester.Chat.OwnerId == request.RequesterId)
		{
			var chatUserByUser = await _context.ChatUsers
				.FirstOrDefaultAsync(c => c.ChatId == request.ChatId && c.UserId == request.RequesterId, cancellationToken);
		
			if (chatUserByUser == null) throw new DbEntityNotFoundException("No user found in chat");
			
			chatUserByUser.CanSendMedia = request.CanSendMedia;

			_context.ChatUsers.Update(chatUserByUser);
			await _context.SaveChangesAsync(cancellationToken);
				
			return new UserDto(chatUserByUser.User);
		}
		
		throw new ForbiddenException("No rights to create user permissions in the chat");
	}
}