using MediatR;
using Messenger.BusinessLogic.Exceptions;
using Messenger.BusinessLogic.Models;
using Messenger.Domain.Enum;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.Channels.Queries;

public class GetChannelQueryHandler : IRequestHandler<GetChannelQuery, ChatDto>
{
	private readonly DatabaseContext _context;

	public GetChannelQueryHandler(DatabaseContext context)
	{
		_context = context;
	}

	public async Task<ChatDto> Handle(GetChannelQuery request, CancellationToken cancellationToken)
	{
		var channel = 
			await (from chat in _context.Chats.AsNoTracking()
					join chatUsers in _context.ChatUsers.AsNoTracking()
						on new {x1 = request.RequesterId, x2 = chat.Id} equals new {x1 = chatUsers.UserId,x2 = chatUsers.ChatId }
						into chatUsersEnumerable
					from chatUsersItem in chatUsersEnumerable.DefaultIfEmpty()
					where chat.Id == request.ChannelId && chat.Type == ChatType.Channel
					select new ChatDto
					{
						Id = chat.Id,
						Name = chat.Name,
						Title = chat.Title,
						Type = chat.Type,
						AvatarLink = chat.AvatarLink,
						MembersCount = chat.ChatUsers.Count,
						CanSendMedia = chatUsersItem != null && chatUsersItem.CanSendMedia,
						IsOwner = chat.OwnerId == request.RequesterId,
						IsMember = chatUsersItem != null,
					})
				.FirstOrDefaultAsync(cancellationToken);
	
		if (channel == null) throw new DbEntityNotFoundException("Channel not found");

		return channel;
	}
}