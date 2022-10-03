using MediatR;
using Messenger.BusinessLogic.Exceptions;
using Messenger.BusinessLogic.Models;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.Conversations.Commands;

public class UpdateConversationCommandHandler : IRequestHandler<UpdateConversationCommand, ChatDto>
{
	private readonly DatabaseContext _context;

	public UpdateConversationCommandHandler(DatabaseContext context)
	{
		_context = context;
	}
	
	public async Task<ChatDto> Handle(UpdateConversationCommand request, CancellationToken cancellationToken)
	{
		var chatUserByRequester = await _context.ChatUsers
			.FirstOrDefaultAsync(r => r.UserId == request.RequesterId && r.ChatId == request.ChatId, cancellationToken);

		if (chatUserByRequester == null)
			throw new DbEntityNotFoundException("No requester in the chat");

		if (chatUserByRequester.Role is { CanChangeChatData: true } || chatUserByRequester.Chat.OwnerId == request.RequesterId)
		{
			if (request.Name != chatUserByRequester.Chat.Name)
			{
				var conversationByName = await _context.Chats
					.FirstOrDefaultAsync(c => c.Name == request.Name, cancellationToken);
			
				if (conversationByName != null && conversationByName.Id != chatUserByRequester.Chat.Id) 
					throw new DbEntityExistsException("Сonference with that name already exists");
			
				chatUserByRequester.Chat.Name = request.Name;
			}
			
			chatUserByRequester.Chat.Title = request.Title;
			
			_context.Chats.Update(chatUserByRequester.Chat);
			await _context.SaveChangesAsync(cancellationToken);
			
			return new ChatDto
			{
				Id = chatUserByRequester.Chat.Id,
				Name = chatUserByRequester.Chat.Name,
				Title = chatUserByRequester.Chat.Title,
				Type = chatUserByRequester.Chat.Type,
				AvatarLink = chatUserByRequester.Chat.AvatarLink,
				MembersCount = chatUserByRequester.Chat.ChatUsers.Count,
				CanSendMedia = chatUserByRequester.CanSendMedia,
				IsOwner = chatUserByRequester.Chat.OwnerId == request.RequesterId,
				IsMember = true,
				MuteDateOfExpire = chatUserByRequester.MuteDateOfExpire,
				BanDateOfExpire = null,
				RoleUser = chatUserByRequester.Role != null ? new RoleUserByChatDto(chatUserByRequester.Role) : null
			};
		}
		
		throw new ForbiddenException("It is forbidden to update someone else's conversation");
	}
}