using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiCommands.Dialogs;

public record CreateDialogCommand(
	Guid RequestorId,
	Guid UserId) 
	: IRequest<Result<ChatDto>>;