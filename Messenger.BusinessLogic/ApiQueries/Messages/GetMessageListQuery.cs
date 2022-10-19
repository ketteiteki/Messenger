using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiQueries.Messages;

public record GetMessageListQuery(
	Guid RequesterId,
	Guid ChatId,
	int Limit,
	DateTime? FromMessageDateTime)
	: IRequest<Result<List<MessageDto>>>;