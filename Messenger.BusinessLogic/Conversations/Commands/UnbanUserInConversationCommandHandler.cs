using MediatR;
using Messenger.BusinessLogic.Exceptions;
using Messenger.BusinessLogic.Models;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.Conversations.Commands;

public class UnbanUserInConversationCommandHandler : IRequestHandler<UnbanUserInConversationCommand, UserDto>
{
	private readonly DatabaseContext _context;

	public UnbanUserInConversationCommandHandler(DatabaseContext context)
	{
		_context = context;
	}
	
	public async Task<UserDto> Handle(UnbanUserInConversationCommand request, CancellationToken cancellationToken)
	{
		var chatUserByRequester = await _context.ChatUsers
			.FirstOrDefaultAsync(r => r.UserId == request.RequesterId && r.ChatId == request.ChatId, cancellationToken);

		if (chatUserByRequester == null)
			throw new DbEntityNotFoundException("No requester in the chat");

		if (chatUserByRequester.Role is { CanBanUser: true } || chatUserByRequester.Chat.OwnerId == request.RequesterId)
		{
			var banUserByChat = await _context.BanUserByChats
				.FirstOrDefaultAsync(r => r.UserId == request.UserId && r.ChatId == request.ChatId, cancellationToken);

			if (banUserByChat == null)
				throw new DbEntityNotFoundException("User is not banned");
			
			_context.BanUserByChats.Remove(banUserByChat);
			await _context.SaveChangesAsync(cancellationToken);
			
			return new UserDto(banUserByChat.User);
		}
		
		throw new ForbiddenException("No rights to unban a user in chat");
	}
}