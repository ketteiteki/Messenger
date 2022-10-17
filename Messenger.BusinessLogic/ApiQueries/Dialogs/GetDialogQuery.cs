using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiQueries.Dialogs;

public record GetDialogQuery(
	Guid RequesterId,
	Guid WithWhomId)
	: IRequest<Result<ChatDto>>;