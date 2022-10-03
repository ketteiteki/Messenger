using MediatR;
using Messenger.BusinessLogic.Exceptions;
using Messenger.BusinessLogic.Models;
using Messenger.Domain.Enum;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.BusinessLogic.Dialogs.Queries;

public class GetDialogQueryHandler : IRequestHandler<GetDialogQuery, ChatDto>
{
	private readonly DatabaseContext _context;

	public GetDialogQueryHandler(DatabaseContext context)
	{
		_context = context;
	}

	public async Task<ChatDto> Handle(GetDialogQuery request, CancellationToken cancellationToken)
	{
		var dialog = await (
				from chatUser1 in _context.ChatUsers.AsNoTracking()
				join chatUser2 in _context.ChatUsers.AsNoTracking()
					on new {x1 = chatUser1.UserId, x2 = chatUser1.ChatId} equals new {x1 = chatUser2.UserId, x2 = chatUser2.ChatId}
				where chatUser1.Chat.Type == ChatType.Dialog &&
				      chatUser1.User.Id == request.RequesterId && chatUser2.User.Id == request.WithWhomId
				select new ChatDto
			{
				Id = chatUser2.Chat.Id,
				Name = chatUser2.Chat.Name,
				Title = chatUser2.Chat.Title,
				Type = chatUser2.Chat.Type,
				AvatarLink = chatUser2.User.AvatarLink,
				MembersCount = 2,
				IsMember = true
			})
			.FirstOrDefaultAsync(cancellationToken);

		if (dialog == null) throw new DbEntityNotFoundException("Conversation not found");

		return dialog;
	}
}