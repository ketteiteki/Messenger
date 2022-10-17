using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiQueries.Messages;

public record GetMessageQuery(
	Guid RequesterId,
	Guid ChatId,
	Guid MessageId
	) : IRequest<Result<MessageDto>>;