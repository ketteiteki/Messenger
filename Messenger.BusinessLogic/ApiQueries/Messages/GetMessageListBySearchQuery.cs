using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiQueries.Messages;

public record GetMessageListBySearchQuery(
		Guid RequesterId,
		Guid ChatId,
		int Limit,
		DateTime? FromMessageDateTime,
		string SearchText) 
	: IRequest<Result<List<MessageDto>>>;