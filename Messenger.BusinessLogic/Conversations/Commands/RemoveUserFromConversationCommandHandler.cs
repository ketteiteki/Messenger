using MediatR;
using Messenger.BusinessLogic.Exceptions;
using Messenger.BusinessLogic.Models;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.Conversations.Commands;

public class RemoveUserFromConversationCommandHandler : IRequestHandler<RemoveUserFromConversationCommand, UserDto>
{
	private readonly DatabaseContext _context;

	public RemoveUserFromConversationCommandHandler(DatabaseContext context)
	{
		_context = context;
	}
	
	public async Task<UserDto> Handle(RemoveUserFromConversationCommand request, CancellationToken cancellationToken)
	{
		var chatUserByRequester = await _context.ChatUsers
			.FirstOrDefaultAsync(r => r.UserId == request.RequesterId && r.ChatId == request.ChatId, cancellationToken);

		if (chatUserByRequester == null)
			throw new DbEntityNotFoundException("No requester in the chat");

		if (chatUserByRequester.Role is { CanAddAndRemoveUserToConversation: true } ||
		    chatUserByRequester.Chat.OwnerId == request.RequesterId)
		{
			var chatUserByUser = await _context.ChatUsers
				.FirstOrDefaultAsync(r => r.UserId == request.RequesterId && r.ChatId == request.ChatId, cancellationToken);

			if (chatUserByUser == null)
				throw new DbEntityNotFoundException("No User in the chat");
			
			_context.ChatUsers.Remove(chatUserByUser);
			await _context.SaveChangesAsync(cancellationToken);
			
			return new UserDto(chatUserByUser.User);
		}
		
		throw new ForbiddenException("You cannot delete a user in someone else's conversation");
	}
}