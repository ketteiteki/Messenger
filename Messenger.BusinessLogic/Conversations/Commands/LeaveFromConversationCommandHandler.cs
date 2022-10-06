using MediatR;
using Messenger.BusinessLogic.Exceptions;
using Messenger.BusinessLogic.Models;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.Conversations.Commands;

public class LeaveFromConversationCommandHandler : IRequestHandler<LeaveFromConversationCommand, ChatDto>
{
	private readonly DatabaseContext _context;

	public LeaveFromConversationCommandHandler(DatabaseContext context)
	{
		_context = context;
	}
	
	public async Task<ChatDto> Handle(LeaveFromConversationCommand request, CancellationToken cancellationToken)
	{
		var chatUser = await _context.ChatUsers
			.Include(c => c.Chat)
			.FirstOrDefaultAsync(c => c.ChatId == request.ChatId && c.UserId == request.RequesterId, cancellationToken);

		if (chatUser == null)
			throw new ForbiddenException("No user found in chat");
		
		_context.ChatUsers.Remove(chatUser);
		await _context.SaveChangesAsync(cancellationToken);

		return new ChatDto
		{
			Id = chatUser.ChatId,
			Name = chatUser.Chat.Name,
			Title = chatUser.Chat.Title,
			Type = chatUser.Chat.Type,
			AvatarLink = chatUser.Chat.AvatarLink,
			CanSendMedia = chatUser.CanSendMedia,
			IsOwner = chatUser.Chat.OwnerId == request.RequesterId,
			IsMember = false,
			MuteDateOfExpire = chatUser.MuteDateOfExpire,
			BanDateOfExpire = null
		};
	}
}