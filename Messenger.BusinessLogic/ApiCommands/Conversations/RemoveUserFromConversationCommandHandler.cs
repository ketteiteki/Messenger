using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiCommands.Conversations;

public class RemoveUserFromConversationCommandHandler : IRequestHandler<RemoveUserFromConversationCommand, Result<UserDto>>
{
	private readonly DatabaseContext _context;

	public RemoveUserFromConversationCommandHandler(DatabaseContext context)
	{
		_context = context;
	}
	
	public async Task<Result<UserDto>> Handle(RemoveUserFromConversationCommand request, CancellationToken cancellationToken)
	{
		var chatUserByRequester = await _context.ChatUsers
			.Include(c => c.Chat)
			.Include(c => c.User)
			.Include(c => c.Role)
			.FirstOrDefaultAsync(r => r.UserId == request.RequesterId && r.ChatId == request.ChatId, cancellationToken);

		if (chatUserByRequester == null)
		{
			return new Result<UserDto>(new DbEntityNotFoundError("No requester in the chat"));
		}
		
		if (chatUserByRequester.Role is { CanAddAndRemoveUserToConversation: true } ||
		    chatUserByRequester.Chat.OwnerId == request.RequesterId)
		{
			var chatUserByUser = await _context.ChatUsers
				.Include(c => c.User)
				.FirstOrDefaultAsync(r => r.UserId == request.UserId && r.ChatId == request.ChatId, cancellationToken);

			if (chatUserByUser == null)
			{
				return new Result<UserDto>(new DbEntityNotFoundError("No User in the chat"));
			}
			
			_context.ChatUsers.Remove(chatUserByUser);
			await _context.SaveChangesAsync(cancellationToken);
			
			return new Result<UserDto>(new UserDto(chatUserByUser.User));
		}
		
		return new Result<UserDto>(new ForbiddenError("You cannot delete a user in someone else's conversation"));
	}
}