using MediatR;
using Messenger.BusinessLogic.Hubs;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Entities;
using Messenger.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiCommands.Conversations;

public class BanUserInConversationCommandHandler : IRequestHandler<BanUserInConversationCommand, Result<UserDto>>
{
	private readonly DatabaseContext _context;
	private readonly IHubContext<ChatHub, IChatHub> _hubContext;

	public BanUserInConversationCommandHandler(DatabaseContext context, IHubContext<ChatHub, IChatHub> hubContext)
	{
		_context = context;
		_hubContext = hubContext;
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
		
		if (chatUserByRequester.Role is { CanBanUser: true } || chatUserByRequester.Chat.OwnerId == request.RequesterId)
		{
			var chatUser = await _context.ChatUsers
				.Include(c => c.User)
				.FirstOrDefaultAsync(b => b.UserId == request.UserId && b.ChatId == request.ChatId, cancellationToken);

			if (chatUser == null)
			{
				return new Result<UserDto>(new DbEntityNotFoundError("User is not in this chat"));
			}

			var banDateOfExpire = DateTime.UtcNow.AddMinutes(request.BanMinutes);
			
			_context.BanUserByChats.Add(new BanUserByChat
			{
				UserId = request.UserId, 
				ChatId = request.ChatId, 
				BanDateOfExpire = banDateOfExpire
			});
			
			_context.ChatUsers.Remove(chatUser);
			await _context.SaveChangesAsync(cancellationToken);

			var notifyBanUserDto = new NotifyBanUserDto(chatId: request.ChatId, banDateOfExpire: banDateOfExpire);
			
			await _hubContext.Clients.User(request.UserId.ToString()).NotifyBanUser(notifyBanUserDto);
			
			return new Result<UserDto>(new UserDto(chatUser.User));
		}

		return new Result<UserDto>(new ForbiddenError("No rights to ban a user in chat"));
	}
}