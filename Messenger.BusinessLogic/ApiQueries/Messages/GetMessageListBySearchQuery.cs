using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiQueries.Messages;

public record GetMessageListBySearchQuery(
		Guid RequestorId,
		Guid ChatId,
		int Limit,
		DateTime? FromMessageDateOfCreate,
		Guid? FromUserId,
		string SearchText) 
	: IRequest<Result<List<MessageDto>>>;