using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiQueries.Chats;

public record GetChatListQuery(
	Guid RequesterId)
	: IRequest<Result<List<ChatDto>>>;