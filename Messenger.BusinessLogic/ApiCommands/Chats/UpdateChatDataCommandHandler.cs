using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Enum;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiCommands.Chats;

public class UpdateChatDataCommandHandler : IRequestHandler<UpdateChatDataCommand, Result<ChatDto>>
{
	private readonly DatabaseContext _context;

	public UpdateChatDataCommandHandler(DatabaseContext context)
	{
		_context = context;
	}
	
	public async Task<Result<ChatDto>> Handle(UpdateChatDataCommand request, CancellationToken cancellationToken)
	{
		var chatUserByRequester = await _context.ChatUsers
			.Include(c => c.Chat)
			.ThenInclude(c => c.Owner)
			.Include(c => c.Role)
			.FirstOrDefaultAsync(c => c.UserId == request.RequesterId &&
			                          c.ChatId == request.ChatId && 
			                          c.Chat.Type != ChatType.Dialog, cancellationToken);

		if (chatUserByRequester == null)
		{
			return new Result<ChatDto>(new DbEntityNotFoundError("No requester in the chat"));
		}
		
		if (chatUserByRequester.Role is { CanChangeChatData: true } ||  chatUserByRequester.Chat.OwnerId == request.RequesterId)
		{
			if (request.Name != chatUserByRequester.Chat.Name)
			{
				var conversationByName = await _context.Chats
					.AsNoTracking()
					.FirstOrDefaultAsync(c => c.Name == request.Name, cancellationToken);

				if (conversationByName != null && conversationByName.Id != chatUserByRequester.Chat.Id)
				{
					return new Result<ChatDto>(new DbEntityExistsError("Chat with that name already exists"));
				}
			
				chatUserByRequester.Chat.Name = request.Name;
			}
			
			chatUserByRequester.Chat.Title = request.Title;
			
			_context.Chats.Update(chatUserByRequester.Chat);
			await _context.SaveChangesAsync(cancellationToken);
			
			return new Result<ChatDto>(
				new ChatDto
				{
					Id =  chatUserByRequester.Chat.Id,
					Name =  chatUserByRequester.Chat.Name,
					Title =  chatUserByRequester.Chat.Title,
					Type =  chatUserByRequester.Chat.Type,
					AvatarLink =  chatUserByRequester.Chat.AvatarLink,
					LastMessageId = chatUserByRequester.Chat.LastMessageId,
					LastMessageText = chatUserByRequester.Chat.LastMessage?.Text,
					LastMessageAuthorDisplayName = chatUserByRequester.Chat.LastMessage != null && 
					                               chatUserByRequester.Chat.LastMessage.Owner != null ?
						chatUserByRequester.Chat.LastMessage.Owner.DisplayName : null,
					LastMessageDateOfCreate = chatUserByRequester.Chat.LastMessage?.DateOfCreate,
					MembersCount =  chatUserByRequester.Chat.ChatUsers.Count,
					CanSendMedia = chatUserByRequester.CanSendMedia,
					IsOwner =  chatUserByRequester.Chat.OwnerId == request.RequesterId,
					IsMember = true,
					MuteDateOfExpire = chatUserByRequester.MuteDateOfExpire,
					BanDateOfExpire = null,
					RoleUser = chatUserByRequester.Role != null ? new RoleUserByChatDto(chatUserByRequester.Role) : null
				});
		}
		
		return new Result<ChatDto>(new ForbiddenError("It is forbidden to update someone else's chat"));
	}
}