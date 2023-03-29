using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiCommands.Conversations;

public class UnbanUserInConversationCommandHandler : IRequestHandler<UnbanUserInConversationCommand, Result<UserDto>>
{
	private readonly DatabaseContext _context;
	private readonly  IBlobServiceSettings _blobServiceSettings;

	public UnbanUserInConversationCommandHandler(
		DatabaseContext context,
		IBlobServiceSettings blobServiceSettings)
	{
		_context = context;
		_blobServiceSettings = blobServiceSettings;
	}
	
	public async Task<Result<UserDto>> Handle(UnbanUserInConversationCommand request, CancellationToken cancellationToken)
	{
		var chatUserByRequester = await _context.ChatUsers
			.Include(c => c.Chat)
			.Include(c => c.Role)
			.FirstOrDefaultAsync(r => r.UserId == request.RequesterId && r.ChatId == request.ChatId, cancellationToken);

		if (chatUserByRequester == null)
		{
			return new Result<UserDto>(new DbEntityNotFoundError("No requester in the chat"));
		}
		
		if ((chatUserByRequester.Role == null &&
		     chatUserByRequester.Chat.OwnerId != request.RequesterId)  || 
		    (chatUserByRequester.Role is { CanBanUser: false } &&
		     chatUserByRequester.Chat.OwnerId != request.RequesterId))
		{
			return new Result<UserDto>(new ForbiddenError("No rights to unban a user in chat"));
		}
		
		var banUserByChat = await _context.BanUserByChats
			.Include(b => b.User)
			.FirstOrDefaultAsync(r => r.UserId == request.UserId && r.ChatId == request.ChatId, cancellationToken);

		if (banUserByChat == null)
		{
			return new Result<UserDto>(new DbEntityNotFoundError("User is not banned"));
		}
			
		_context.BanUserByChats.Remove(banUserByChat);
		
		await _context.SaveChangesAsync(cancellationToken);
			
		var avatarLink = banUserByChat.User.AvatarFileName != null
			? $"{_blobServiceSettings.MessengerBlobAccess}/{banUserByChat.User.AvatarFileName}"
			: null;
		
		var userDto = new UserDto(
			banUserByChat.User.Id,
			banUserByChat.User.DisplayName,
			banUserByChat.User.Nickname,
			banUserByChat.User.Bio,
			avatarLink);
		
		return new Result<UserDto>(userDto);
		
	}
}