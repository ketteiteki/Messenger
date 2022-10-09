using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiQueries.Messages;

public record GetMessageListQuery(
	Guid RequestorId,
	Guid ChatId,
	int Limit,
	DateTime? FromMessageDateTime)
	: IRequest<Result<List<MessageDto>>>;