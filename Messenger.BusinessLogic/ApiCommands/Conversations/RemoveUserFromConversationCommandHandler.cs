using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Persistence;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiCommands.Conversations;

public class RemoveUserFromConversationCommandHandler : IRequestHandler<RemoveUserFromConversationCommand, Result<UserDto>>
{
	private readonly DatabaseContext _context;
	private readonly IBlobServiceSettings _blobServiceSettings;

	public RemoveUserFromConversationCommandHandler(
		DatabaseContext context,
		IBlobServiceSettings blobServiceSettings)
	{
		_context = context;
		_blobServiceSettings = blobServiceSettings;
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
		
		if ((chatUserByRequester.Role == null &&
		     chatUserByRequester.Chat.OwnerId != request.RequesterId)  || 
		    (chatUserByRequester.Role is { CanAddAndRemoveUserToConversation: false } &&
		     chatUserByRequester.Chat.OwnerId != request.RequesterId))
		{
			return new Result<UserDto>(new ForbiddenError("You cannot delete a user in someone else's conversation"));
		}
		
		var chatUserByUser = await _context.ChatUsers
			.Include(c => c.User)
			.FirstOrDefaultAsync(r => r.UserId == request.UserId && r.ChatId == request.ChatId, cancellationToken);

		if (chatUserByUser == null)
		{
			return new Result<UserDto>(new DbEntityNotFoundError("No User in the chat"));
		}
			
		_context.ChatUsers.Remove(chatUserByUser);
		
		await _context.SaveChangesAsync(cancellationToken);
			
		var avatarLink = chatUserByUser.User.AvatarFileName != null
			? $"{_blobServiceSettings.MessengerBlobAccess}/{chatUserByUser.User.AvatarFileName}"
			: null;
		
		var userDto = new UserDto(
			chatUserByUser.User.Id,
			chatUserByUser.User.DisplayName,
			chatUserByUser.User.Nickname,
			chatUserByUser.User.Bio,
			avatarLink);
		
		return new Result<UserDto>(userDto);
		
	}
}