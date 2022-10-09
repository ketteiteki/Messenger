using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiQueries.Chats;

public record GetChatQuery(
	Guid RequestorId,
	Guid ChatId) 
	: IRequest<Result<ChatDto>>;