using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiCommands.Conversations;

public class LeaveFromConversationCommandHandler : IRequestHandler<LeaveFromConversationCommand, Result<ChatDto>>
{
	private readonly DatabaseContext _context;

	public LeaveFromConversationCommandHandler(DatabaseContext context)
	{
		_context = context;
	}
	
	public async Task<Result<ChatDto>> Handle(LeaveFromConversationCommand request, CancellationToken cancellationToken)
	{
		var chatUser = await _context.ChatUsers
			.Include(c => c.Chat)
			.FirstOrDefaultAsync(c => c.ChatId == request.ChatId && c.UserId == request.RequestorId, cancellationToken);

		if (chatUser == null)
			return new Result<ChatDto>(new ForbiddenError("No user found in chat"));
		
		_context.ChatUsers.Remove(chatUser);
		await _context.SaveChangesAsync(cancellationToken);

		return new Result<ChatDto>(
			new ChatDto
			{
				Id = chatUser.ChatId,
				Name = chatUser.Chat.Name,
				Title = chatUser.Chat.Title,
				Type = chatUser.Chat.Type,
				AvatarLink = chatUser.Chat.AvatarLink,
				CanSendMedia = chatUser.CanSendMedia,
				IsOwner = chatUser.Chat.OwnerId == request.RequestorId,
				IsMember = false,
				MuteDateOfExpire = chatUser.MuteDateOfExpire,
				BanDateOfExpire = null
			});
	}
}