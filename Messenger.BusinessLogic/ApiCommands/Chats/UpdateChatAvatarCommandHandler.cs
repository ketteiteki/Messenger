using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Enum;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiCommands.Chats;

public class UpdateChatAvatarCommandHandler : IRequestHandler<UpdateChatAvatarCommand, Result<ChatDto>>
{
	private readonly DatabaseContext _context;
	private readonly IBlobService _blobService;

	public UpdateChatAvatarCommandHandler(
		DatabaseContext context,
		IBlobService blobService)
	{
		_context = context;
		_blobService = blobService;
	}
	
	public async Task<Result<ChatDto>> Handle(UpdateChatAvatarCommand request, CancellationToken cancellationToken)
	{
		var chatUserByRequester = await _context.ChatUsers
			.Include(c => c.Chat)
			.ThenInclude(c => c.Owner)
			.Include(c => c.Role)
			.FirstOrDefaultAsync(c => 
				c.UserId == request.RequesterId && 
				c.ChatId == request.ChatId &&
				c.Chat.Type != ChatType.Dialog, cancellationToken);

		if (chatUserByRequester == null)
		{
			return new Result<ChatDto>(new DbEntityNotFoundError("No requester in the chat"));
		}

		if ((chatUserByRequester.Role == null &&
		     chatUserByRequester.Chat.OwnerId != request.RequesterId)  || 
		    (chatUserByRequester.Role is { CanChangeChatData: false } &&
		     chatUserByRequester.Chat.OwnerId != request.RequesterId))
		{
			return new Result<ChatDto>(new ForbiddenError("It is forbidden to update someone else's chat"));
		}

		if (chatUserByRequester.Chat.AvatarLink != null)
		{
			var avatarFileName = chatUserByRequester.Chat.AvatarLink.Split("/")[^1];

			await _blobService.DeleteBlobAsync(avatarFileName);
			
			chatUserByRequester.Chat.UpdateAvatarLink(null);
		}
			
		if (request.AvatarFile != null)
		{
			var avatarLink = await _blobService.UploadFileBlobAsync(request.AvatarFile);
			
			chatUserByRequester.Chat.UpdateAvatarLink(avatarLink);
		}
			
			
		_context.Chats.Update(chatUserByRequester.Chat);
		await _context.SaveChangesAsync(cancellationToken);

		var chatDto = new ChatDto
		{
			Id = chatUserByRequester.ChatId,
			Name = chatUserByRequester.Chat.Name,
			Title = chatUserByRequester.Chat.Title,
			Type = chatUserByRequester.Chat.Type,
			AvatarLink = chatUserByRequester.Chat.AvatarLink,
			LastMessageId = chatUserByRequester.Chat.LastMessageId,
			LastMessageText = chatUserByRequester.Chat.LastMessage?.Text,
			LastMessageAuthorDisplayName =
				chatUserByRequester.Chat.LastMessage is { Owner: { } }
				? chatUserByRequester.Chat.LastMessage.Owner.DisplayName
				: null,
			LastMessageDateOfCreate = chatUserByRequester.Chat.LastMessage?.DateOfCreate,
			IsOwner = chatUserByRequester.Chat.OwnerId == request.RequesterId,
			IsMember = true,
			MuteDateOfExpire = chatUserByRequester.MuteDateOfExpire,
			BanDateOfExpire = null,
			RoleUser = chatUserByRequester.Role != null ? new RoleUserByChatDto(chatUserByRequester.Role) : null
		};
			
		return new Result<ChatDto>(chatDto);
	}
}