using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Messenger.Domain.Enum;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.ApiQueries.Channels;

public class GetChannelQueryHandler : IRequestHandler<GetChannelQuery, Result<ChatDto>>
{
	private readonly DatabaseContext _context;

	public GetChannelQueryHandler(DatabaseContext context)
	{
		_context = context;
	}

	public async Task<Result<ChatDto>> Handle(GetChannelQuery request, CancellationToken cancellationToken)
	{
		var channel = 
			await (from chat in _context.Chats.AsNoTracking()
					join chatUsers in _context.ChatUsers.AsNoTracking()
						on new {x1 = request.RequestorId, x2 = chat.Id} equals new {x1 = chatUsers.UserId,x2 = chatUsers.ChatId }
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
						IsOwner = chat.OwnerId == request.RequestorId,
						IsMember = chatUsersItem != null,
					})
				.FirstOrDefaultAsync(cancellationToken);

		if (channel == null) return new Result<ChatDto>(new DbEntityNotFoundError("Channel not found"));

		return new Result<ChatDto>(channel);
	}
}