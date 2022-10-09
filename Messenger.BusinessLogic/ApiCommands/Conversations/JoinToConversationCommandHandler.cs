using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Entities;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiCommands.Conversations;

public class JoinToConversationCommandHandler : IRequestHandler<JoinToConversationCommand, Result<ChatDto>>
{
	private readonly DatabaseContext _context;

	public JoinToConversationCommandHandler(DatabaseContext context)
	{
		_context = context;
	}
	
	public async Task<Result<ChatDto>> Handle(JoinToConversationCommand request, CancellationToken cancellationToken)
	{
		var chatUser = await _context.ChatUsers
			.FirstOrDefaultAsync(c => c.UserId == request.RequestorId && c.ChatId == request.ChatId, cancellationToken);

		if (chatUser != null)
			return new Result<ChatDto>(new DbEntityExistsError("User already exists in the conversation"));

		var banUserByChat = await _context.BanUserByChats
			.FirstOrDefaultAsync(b => b.UserId == request.RequestorId && b.ChatId == request.ChatId, cancellationToken);

		if (banUserByChat != null)
			return new Result<ChatDto>(
				new ForbiddenError($"You are banned in the chat. Unban date: {banUserByChat.BanDateOfExpire}"));
		
		var newChatUser = new ChatUser { UserId = request.RequestorId, ChatId = request.ChatId };
		
		_context.ChatUsers.Add(newChatUser);
		await _context.SaveChangesAsync(cancellationToken);

		await _context.Entry(newChatUser).Reference(c => c.Chat).LoadAsync(cancellationToken);
		
		return new Result<ChatDto>(
			new ChatDto
			{
				Id = newChatUser.ChatId,
				Name = newChatUser.Chat.Name,
				Title = newChatUser.Chat.Title,
				Type = newChatUser.Chat.Type,
				AvatarLink = newChatUser.Chat.AvatarLink,
				IsOwner = newChatUser.Chat.OwnerId == request.RequestorId,
				IsMember = true,
				MuteDateOfExpire = newChatUser.MuteDateOfExpire,
				BanDateOfExpire = null,
			});
	}
}
