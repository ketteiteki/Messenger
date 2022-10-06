using MediatR;
using Messenger.BusinessLogic.Exceptions;
using Messenger.BusinessLogic.Models;
using Messenger.Domain.Entities;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.Conversations.Commands;

public class BanUserInConversationCommandHandler : IRequestHandler<BanUserInConversationCommand, UserDto>
{
	private readonly DatabaseContext _context;

	public BanUserInConversationCommandHandler(DatabaseContext context)
	{
		_context = context;
	}
	
	public async Task<UserDto> Handle(BanUserInConversationCommand request, CancellationToken cancellationToken)
	{
		if (DateTime.UtcNow > request.BanDateOfExpire)
			throw new BadRequestException("The ban time must be longer than the current time");

		var chatUserByRequester = await _context.ChatUsers
			.Include(c => c.Role)
			.Include(c => c.Chat)
			.FirstOrDefaultAsync(c => c.UserId == request.RequesterId && c.ChatId == request.ChatId, cancellationToken);

		if (chatUserByRequester == null)
			throw new ForbiddenException("No requester in the chat");
		
		if (chatUserByRequester.Role is { CanBanUser: true } || chatUserByRequester.Chat.OwnerId == request.RequesterId)
		{
			var chatUser = await _context.ChatUsers
				.Include(c => c.User)
				.FirstOrDefaultAsync(b => b.UserId == request.UserId && b.ChatId == request.ChatId, cancellationToken);

			if (chatUser == null) throw new ForbiddenException("User is not in this chat");
			
			_context.BanUserByChats.Add(
				new BanUserByChat {UserId = request.UserId, ChatId = request.ChatId, BanDateOfExpire = request.BanDateOfExpire});
			_context.ChatUsers.Remove(chatUser);
			await _context.SaveChangesAsync(cancellationToken);
			
			return new UserDto(chatUser.User);
		}
		
		throw new ForbiddenException("No rights to ban a user in chat");
	}
}