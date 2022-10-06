using MediatR;
using Messenger.BusinessLogic.Exceptions;
using Messenger.BusinessLogic.Models;
using Messenger.Domain.Entities;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.Conversations.Commands;

public class JoinToConversationCommandHandler : IRequestHandler<JoinToConversationCommand, ChatDto>
{
	private readonly DatabaseContext _context;

	public JoinToConversationCommandHandler(DatabaseContext context)
	{
		_context = context;
	}
	
	public async Task<ChatDto> Handle(JoinToConversationCommand request, CancellationToken cancellationToken)
	{
		var chatUser = await _context.ChatUsers
			.FirstOrDefaultAsync(c => c.UserId == request.RequesterId && c.ChatId == request.ChatId, cancellationToken);
		
		if (chatUser != null)
			throw new InvalidOperationException("User already exists in the conversation");

		var banUserByChat = await _context.BanUserByChats
			.FirstOrDefaultAsync(b => b.UserId == request.RequesterId && b.ChatId == request.ChatId, cancellationToken);

		if (banUserByChat != null)
			throw new ForbiddenException($"You are banned in the chat. Unban date: {banUserByChat.BanDateOfExpire}");
		
		var newChatUser = new ChatUser { UserId = request.RequesterId, ChatId = request.ChatId };
		
		_context.ChatUsers.Add(newChatUser);
		await _context.SaveChangesAsync(cancellationToken);

		await _context.Entry(newChatUser).Reference(c => c.Chat).LoadAsync(cancellationToken);
		
		return new ChatDto
		{
			Id = newChatUser.ChatId,
			Name = newChatUser.Chat.Name,
			Title = newChatUser.Chat.Title,
			Type = newChatUser.Chat.Type,
			AvatarLink = newChatUser.Chat.AvatarLink,
			IsOwner = newChatUser.Chat.OwnerId == request.RequesterId,
			IsMember = true,
			MuteDateOfExpire = newChatUser.MuteDateOfExpire,
			BanDateOfExpire = null,
		};
	}
}
