using MediatR;
using Messenger.BusinessLogic.Exceptions;
using Messenger.BusinessLogic.Models;
using Messenger.Domain.Entities;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.Conversations.Commands;

public class AddUserToConversationCommandHandler : IRequestHandler<AddUserToConversationCommand, UserDto>
{
	private readonly DatabaseContext _context;

	public AddUserToConversationCommandHandler(DatabaseContext context)
	{
		_context = context;
	}
	
	public async Task<UserDto> Handle(AddUserToConversationCommand request, CancellationToken cancellationToken)
	{
		var chatUserByRequester = await _context.ChatUsers
			.FirstOrDefaultAsync(c => c.UserId == request.RequestorId && c.ChatId == request.ChatId, cancellationToken);	
		
		if (chatUserByRequester == null) throw new ForbiddenException("No requester in the chat");

		if (chatUserByRequester.Role is { CanAddAndRemoveUserToConversation: true } ||
		    chatUserByRequester.ChatId == request.RequestorId)
		{
			var user = await _context.Users
				.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

			if (user == null) throw new DbEntityNotFoundException("User not found");
			
			if (user.ChatUsers.Any(c => c.UserId == request.UserId && c.ChatId == request.ChatId)) 
				throw new DbEntityExistsException("User is already in the chat");
			
			_context.ChatUsers.Add(new ChatUser {UserId = request.UserId, ChatId = request.ChatId});
			await _context.SaveChangesAsync(cancellationToken);
			
			return new UserDto(user);
		}
		
		throw new ForbiddenException("Not right to add a user to the conversation");
	}
}