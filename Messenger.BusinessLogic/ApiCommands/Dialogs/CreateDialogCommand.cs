using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiCommands.Dialogs;

public record CreateDialogCommand(
	Guid RequesterId,
	Guid UserId) 
	: IRequest<Result<ChatDto>>;