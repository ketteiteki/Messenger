using MediatR;
using Messenger.BusinessLogic.Hubs;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Entities;
using Messenger.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiCommands.Conversations;

public class AddUserToConversationCommandHandler : IRequestHandler<AddUserToConversationCommand, Result<UserDto>>
{
	private readonly DatabaseContext _context;
	private readonly IHubContext<ChatHub, IChatHub> _hubContext;

	public AddUserToConversationCommandHandler(DatabaseContext context, IHubContext<ChatHub, IChatHub> hubContext)
	{
		_context = context;
		_hubContext = hubContext;
	}
	
	public async Task<Result<UserDto>> Handle(AddUserToConversationCommand request, CancellationToken cancellationToken)
	{
		var chatUserByRequester = await _context.ChatUsers
			.Include(c => c.Role)
			.Include(c => c.Chat)
			.ThenInclude(c => c.LastMessage)
			.ThenInclude(m => m.Owner)
			.FirstOrDefaultAsync(c => c.UserId == request.RequesterId && c.ChatId == request.ChatId, cancellationToken);

		if (chatUserByRequester == null)
		{
			return new Result<UserDto>(new ForbiddenError("No requester in the chat"));
		}
		
		if ((chatUserByRequester.Role == null &&
		     chatUserByRequester.Chat.OwnerId != request.RequesterId)  || 
		    (chatUserByRequester.Role is { CanAddAndRemoveUserToConversation: false } &&
		     chatUserByRequester.Chat.OwnerId != request.RequesterId))
		{
			return new Result<UserDto>(new ForbiddenError("Not right to add a user to the conversation"));
		}
		
		var user = await _context.Users
			.AsNoTracking()
			.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

		if (user == null)
		{
			return new Result<UserDto>(new  DbEntityNotFoundError("User not found"));
		}
			
		var isChatUserByUserExists = await _context.ChatUsers
			.AnyAsync(c => c.UserId == request.UserId && c.ChatId == request.ChatId, cancellationToken);

		if (isChatUserByUserExists)
		{
			return new Result<UserDto>(new DbEntityExistsError("User already exists in conversation"));
		}

		var isUserAlreadyInChat =
			await _context.ChatUsers.AnyAsync(c => c.UserId == request.UserId && c.ChatId == request.ChatId, cancellationToken);
		
		if (isUserAlreadyInChat)
		{
			return new Result<UserDto>(new  DbEntityExistsError("User is already in the chat"));
		}

		var chatUser = new ChatUserEntity(
			request.UserId,
			request.ChatId,
			canSendMedia: true, 
			muteDateOfExpire: null);
		
		_context.ChatUsers.Add(chatUser);
		
		await _context.SaveChangesAsync(cancellationToken);

		var usersWithRole = await _context.ChatUsers
			.Include(c => c.Role)
			.Where(c => c.ChatId == request.ChatId && c.Role != null)
			.Select(c => new RoleUserByChatDto(c.Role))
			.ToListAsync(cancellationToken);

		var memberCountChat = await _context.ChatUsers.Where(c => c.ChatId == request.ChatId).CountAsync(cancellationToken);
			
		var chatDto = new ChatDto
		{
			Id = chatUserByRequester.Chat.Id,
			Name = chatUserByRequester.Chat.Name,
			Title = chatUserByRequester.Chat.Title,
			Type = chatUserByRequester.Chat.Type,
			AvatarLink = chatUserByRequester.Chat.AvatarLink,
			LastMessageId = chatUserByRequester.Chat.LastMessage?.Id,
			LastMessageText = chatUserByRequester.Chat.LastMessage?.Text,
			LastMessageAuthorDisplayName = chatUserByRequester.Chat.LastMessage?.Owner?.DisplayName,
			LastMessageDateOfCreate = chatUserByRequester.Chat.LastMessage?.DateOfCreate,
			MembersCount = memberCountChat,
			CanSendMedia = true,
			IsOwner = chatUserByRequester.Chat.OwnerId == request.UserId,
			IsMember = true,
			UsersWithRole = usersWithRole
		};
			
		await _hubContext.Clients.User(request.UserId.ToString()).CreateChatForUserAfterAddUserInChat(chatDto);
			
		return new Result<UserDto>(new UserDto(user));
		
	}
}