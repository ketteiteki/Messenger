using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiCommands.Conversations;

public class UnbanUserInConversationCommandHandler : IRequestHandler<UnbanUserInConversationCommand, Result<UserDto>>
{
	private readonly DatabaseContext _context;

	public UnbanUserInConversationCommandHandler(DatabaseContext context)
	{
		_context = context;
	}
	
	public async Task<Result<UserDto>> Handle(UnbanUserInConversationCommand request, CancellationToken cancellationToken)
	{
		var chatUserByRequester = await _context.ChatUsers
			.Include(c => c.Chat)
			.Include(c => c.Role)
			.FirstOrDefaultAsync(r => r.UserId == request.RequesterId && r.ChatId == request.ChatId, cancellationToken);

		if (chatUserByRequester == null)
			return new Result<UserDto>(new DbEntityNotFoundError("No requester in the chat"));

		if (chatUserByRequester.Role is { CanBanUser: true } || chatUserByRequester.Chat.OwnerId == request.RequesterId)
		{
			var banUserByChat = await _context.BanUserByChats
				.Include(b => b.User)
				.FirstOrDefaultAsync(r => r.UserId == request.UserId && r.ChatId == request.ChatId, cancellationToken);

			if (banUserByChat == null)
				return new Result<UserDto>(new DbEntityNotFoundError("User is not banned"));
			
			_context.BanUserByChats.Remove(banUserByChat);
			await _context.SaveChangesAsync(cancellationToken);
			
			return new Result<UserDto>(new UserDto(banUserByChat.User));
		}
		
		return new Result<UserDto>(new ForbiddenError("No rights to unban a user in chat"));
	}
}