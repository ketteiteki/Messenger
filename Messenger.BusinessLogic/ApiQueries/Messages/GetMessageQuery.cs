using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiQueries.Messages;

public record GetMessageQuery(
	Guid RequestorId,
	Guid ChatId,
	Guid MessageId
	) : IRequest<Result<MessageDto>>;