using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Entities;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiCommands.Conversations;

public class BanUserInConversationCommandHandler : IRequestHandler<BanUserInConversationCommand, Result<UserDto>>
{
	private readonly DatabaseContext _context;

	public BanUserInConversationCommandHandler(DatabaseContext context)
	{
		_context = context;
	}
	
	public async Task<Result<UserDto>> Handle(BanUserInConversationCommand request, CancellationToken cancellationToken)
	{
		if (DateTime.UtcNow > request.BanDateOfExpire)
			return new Result<UserDto>(new BadRequestError("The ban time must be longer than the current time"));

		var chatUserByRequestor = await _context.ChatUsers
			.Include(c => c.Role)
			.Include(c => c.Chat)
			.FirstOrDefaultAsync(c => c.UserId == request.RequesterId && c.ChatId == request.ChatId, cancellationToken);

		if (chatUserByRequestor == null)
			return new Result<UserDto>(new ForbiddenError("No requestor in the chat"));
		
		if (chatUserByRequestor.Role is { CanBanUser: true } || chatUserByRequestor.Chat.OwnerId == request.RequesterId)
		{
			var chatUser = await _context.ChatUsers
				.Include(c => c.User)
				.FirstOrDefaultAsync(b => b.UserId == request.UserId && b.ChatId == request.ChatId, cancellationToken);

			if (chatUser == null) 
				return new Result<UserDto>(new ForbiddenError("User is not in this chat"));
			
			_context.BanUserByChats.Add(
				new BanUserByChat {UserId = request.UserId, ChatId = request.ChatId, BanDateOfExpire = request.BanDateOfExpire});
			_context.ChatUsers.Remove(chatUser);
			await _context.SaveChangesAsync(cancellationToken);
			
			return new Result<UserDto>(new UserDto(chatUser.User));
		}

		return new Result<UserDto>(new ForbiddenError("No rights to ban a user in chat"));
	}
}