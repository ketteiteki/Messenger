using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiQueries.Chats;

public record GetChatListQuery(
	Guid RequestorId)
	: IRequest<Result<List<ChatDto>>>;