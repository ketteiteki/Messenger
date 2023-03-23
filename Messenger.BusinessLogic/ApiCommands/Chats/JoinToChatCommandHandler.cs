using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Entities;
using Messenger.Domain.Enum;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiCommands.Chats;

public class JoinToChatCommandHandler : IRequestHandler<JoinToChatCommand, Result<ChatDto>>
{
	private readonly DatabaseContext _context;
	private readonly IBlobServiceSettings _blobServiceSettings;

	public JoinToChatCommandHandler(
		DatabaseContext context, 
		IBlobServiceSettings blobServiceSettings)
	{
		_context = context;
		_blobServiceSettings = blobServiceSettings;
	}

	public async Task<Result<ChatDto>> Handle(JoinToChatCommand request, CancellationToken cancellationToken)
	{
		var chat = await _context.Chats
			.Where(c => c.Type != ChatType.Dialog)
			.FirstOrDefaultAsync(c => c.Id == request.ChatId, cancellationToken);

		if (chat == null)
		{
			return new Result<ChatDto>(new DbEntityNotFoundError("Chat not found"));
		}
		
		var isChatUserExists = await _context.ChatUsers.AnyAsync(c => 
			c.UserId == request.RequesterId && c.ChatId == request.ChatId, cancellationToken);

		if (isChatUserExists)
		{
			return new Result<ChatDto>(new DbEntityExistsError("User already exists in the chat"));
		}

		var banUserByChat = await _context.BanUserByChats
			.FirstOrDefaultAsync(b => b.UserId == request.RequesterId && b.ChatId == request.ChatId, cancellationToken);

		if (banUserByChat != null)
		{
			return new Result<ChatDto>(
				new ForbiddenError($"You are banned in the chat. Unban date: {banUserByChat.BanDateOfExpire}"));
		}
		
		var newChatUser = new ChatUserEntity(
			request.RequesterId,
			request.ChatId,
			canSendMedia: true,
			muteDateOfExpire: null);
		
		_context.ChatUsers.Add(newChatUser);
		await _context.SaveChangesAsync(cancellationToken);

		var memberCount = await _context.ChatUsers.Where(c => c.ChatId == request.ChatId).CountAsync(cancellationToken);

		var avatarLink = chat.AvatarFileName != null ?
			$"{_blobServiceSettings.MessengerBlobAccess}/{chat.AvatarFileName}"
			: null;
		
		var chatDto = new ChatDto
		{
			Id = chat.Id,
			Name = chat.Name,
			Title = chat.Title,
			Type = chat.Type,
			AvatarLink = avatarLink,
			LastMessageId = chat.LastMessageId,
			LastMessageText = chat.LastMessage?.Text,
			LastMessageAuthorDisplayName = 
				chat.LastMessage is { Owner: { } }
				? chat.LastMessage.Owner.DisplayName
				: null,
			LastMessageDateOfCreate = chat.LastMessage?.DateOfCreate,
			IsOwner = chat.OwnerId == request.RequesterId,
			IsMember = true,
			MembersCount = memberCount,
			MuteDateOfExpire = newChatUser.MuteDateOfExpire,
			BanDateOfExpire = null,
		};
		
		return new Result<ChatDto>(chatDto);
	}
}