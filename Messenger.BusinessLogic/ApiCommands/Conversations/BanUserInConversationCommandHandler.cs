using MediatR;
using Messenger.Application.Interfaces;
using Messenger.BusinessLogic.Hubs;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Entities;
using Messenger.Persistence;
using Messenger.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiCommands.Conversations;

public class BanUserInConversationCommandHandler : IRequestHandler<BanUserInConversationCommand, Result<UserDto>>
{
	private readonly DatabaseContext _context;
	private readonly IHubContext<ChatHub, IChatHub> _hubContext;
	private readonly IBlobServiceSettings _blobServiceSettings;

	public BanUserInConversationCommandHandler(
		DatabaseContext context,
		IHubContext<ChatHub, IChatHub> hubContext,
		IBlobServiceSettings blobServiceSettings)
	{
		_context = context;
		_hubContext = hubContext;
		_blobServiceSettings = blobServiceSettings;
	}
	
	public async Task<Result<UserDto>> Handle(BanUserInConversationCommand request, CancellationToken cancellationToken)
	{
		if (request.BanMinutes <= 0)
		{
			return new Result<UserDto>(new BadRequestError("The ban minutes must be greater than 0"));
		}

		var chatUserByRequester = await _context.ChatUsers
			.Include(c => c.Role)
			.Include(c => c.Chat)
			.FirstOrDefaultAsync(c => c.UserId == request.RequesterId && c.ChatId == request.ChatId, cancellationToken);

		if (chatUserByRequester == null)
		{
			return new Result<UserDto>(new ForbiddenError("No requester in the chat"));
		}
		
		if ((chatUserByRequester.Role == null &&
		     chatUserByRequester.Chat.OwnerId != request.RequesterId)  || 
		    (chatUserByRequester.Role is { CanBanUser: false } &&
		     chatUserByRequester.Chat.OwnerId != request.RequesterId))
		{
			return new Result<UserDto>(new ForbiddenError("No rights to ban a user in chat"));
		}
		
		var chatUser = await _context.ChatUsers
			.Include(c => c.User)
			.FirstOrDefaultAsync(b => b.UserId == request.UserId && b.ChatId == request.ChatId, cancellationToken);

		if (chatUser == null)
		{
			return new Result<UserDto>(new DbEntityNotFoundError("User is not in this chat"));
		}

		var banDateOfExpire = DateTime.UtcNow.AddMinutes(request.BanMinutes);

		var banUserByChat = new BanUserByChatEntity(request.UserId, request.ChatId, banDateOfExpire);

		_context.BanUserByChats.Add(banUserByChat);
		_context.ChatUsers.Remove(chatUser);
		
		await _context.SaveChangesAsync(cancellationToken);

		var notifyBanUserDto = new NotifyBanUserDto(request.ChatId, banDateOfExpire);
			
		await _hubContext.Clients.User(request.UserId.ToString()).NotifyBanUser(notifyBanUserDto);

		var avatarLink = chatUser.User.AvatarFileName != null
			? $"{_blobServiceSettings.MessengerBlobAccess}/{chatUser.User.AvatarFileName}"
			: null;
		
		var userDto = new UserDto(
			chatUser.User.Id,
			chatUser.User.DisplayName,
			chatUser.User.Nickname,
			chatUser.User.Bio,
			avatarLink);
			
		return new Result<UserDto>(userDto);
	}
}