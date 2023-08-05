using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Enums;
using Messenger.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiCommands.Chats;

public class UpdateChatDataCommandHandler : IRequestHandler<UpdateChatDataCommand, Result<ChatDto>>
{
	private readonly DatabaseContext _context;
	private readonly IBlobServiceSettings _blobServiceSettings;

	public UpdateChatDataCommandHandler(
		DatabaseContext context,
		IBlobServiceSettings blobServiceSettings)
	{
		_context = context;
		_blobServiceSettings = blobServiceSettings;
	}
	
	public async Task<Result<ChatDto>> Handle(UpdateChatDataCommand request, CancellationToken cancellationToken)
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
		
		if (request.Name != chatUserByRequester.Chat.Name)
		{
			var conversationByName = await _context.Chats
				.AsNoTracking()
				.FirstOrDefaultAsync(c => c.Name == request.Name, cancellationToken);

			if (conversationByName != null && conversationByName.Id != chatUserByRequester.Chat.Id)
			{
				return new Result<ChatDto>(new DbEntityExistsError("Chat with that name already exists"));
			}
			
			chatUserByRequester.Chat.UpdateName(request.Name);
		}
			
		chatUserByRequester.Chat.UpdateTitle(request.Title);
			
		_context.Chats.Update(chatUserByRequester.Chat);
		await _context.SaveChangesAsync(cancellationToken);
			
		var avatarLink = chatUserByRequester.Chat.AvatarFileName != null ?
			$"{_blobServiceSettings.MessengerBlobAccess}/{chatUserByRequester.Chat.AvatarFileName}"
			: null;
		
		return new Result<ChatDto>(
			new ChatDto
			{
				Id =  chatUserByRequester.Chat.Id,
				Name =  chatUserByRequester.Chat.Name,
				Title =  chatUserByRequester.Chat.Title,
				Type =  chatUserByRequester.Chat.Type,
				AvatarLink =  avatarLink,
				LastMessageId = chatUserByRequester.Chat.LastMessageId,
				LastMessageText = chatUserByRequester.Chat.LastMessage?.Text,
				LastMessageAuthorDisplayName = 
					chatUserByRequester.Chat.LastMessage is { Owner: { } } ?
					chatUserByRequester.Chat.LastMessage.Owner.DisplayName : null,
				LastMessageDateOfCreate = chatUserByRequester.Chat.LastMessage?.DateOfCreate,
				MembersCount =  chatUserByRequester.Chat.ChatUsers.Count,
				CanSendMedia = chatUserByRequester.CanSendMedia,
				OwnerId = chatUserByRequester.Chat.OwnerId,
				IsOwner =  chatUserByRequester.Chat.OwnerId == request.RequesterId,
				IsMember = true,
				MuteDateOfExpire = chatUserByRequester.MuteDateOfExpire,
				BanDateOfExpire = null,
				RoleUser = chatUserByRequester.Role != null ? new RoleUserByChatDto(chatUserByRequester.Role) : null
			});
		
	}
}