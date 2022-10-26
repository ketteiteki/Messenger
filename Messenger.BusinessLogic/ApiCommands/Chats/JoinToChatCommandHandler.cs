using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Entities;
using Messenger.Domain.Enum;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiCommands.Chats;

public class JoinToChatCommandHandler : IRequestHandler<JoinToChatCommand, Result<ChatDto>>
{
	private DatabaseContext _context;

	public JoinToChatCommandHandler(DatabaseContext context)
	{
		_context = context;
	}

	public async Task<Result<ChatDto>> Handle(JoinToChatCommand request, CancellationToken cancellationToken)
	{
		var conversation = await _context.Chats
			.Where(c => c.Type != ChatType.Dialog)
			.FirstOrDefaultAsync(c => c.Id == request.ChatId, cancellationToken);

		if (conversation == null) return new Result<ChatDto>(new DbEntityNotFoundError("Chat not found"));
		
		var chatUser = await _context.ChatUsers
			.FirstOrDefaultAsync(c => c.UserId == request.RequesterId && c.ChatId == request.ChatId, cancellationToken);

		if (chatUser != null)
			return new Result<ChatDto>(new DbEntityExistsError("User already exists in the chat"));

		var banUserByChat = await _context.BanUserByChats
			.FirstOrDefaultAsync(b => b.UserId == request.RequesterId && b.ChatId == request.ChatId, cancellationToken);

		if (banUserByChat != null)
			return new Result<ChatDto>(
				new ForbiddenError($"You are banned in the chat. Unban date: {banUserByChat.BanDateOfExpire}"));
		
		var newChatUser = new ChatUser { UserId = request.RequesterId, ChatId = request.ChatId };
		
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
				IsOwner = newChatUser.Chat.OwnerId == request.RequesterId,
				IsMember = true,
				MuteDateOfExpire = newChatUser.MuteDateOfExpire,
				BanDateOfExpire = null,
			});
	}
}