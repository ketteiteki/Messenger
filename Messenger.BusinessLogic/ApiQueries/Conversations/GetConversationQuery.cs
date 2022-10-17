using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiQueries.Conversations;

public record GetConversationQuery(
		Guid RequesterId,
		Guid ChatId)
	: IRequest<Result<ChatDto>>;
