using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiCommands.Dialogs;

public record DeleteDialogCommand(
		Guid RequesterId,
		Guid ChatId,
		bool IsForBoth) 
	: IRequest<Result<ChatDto>>;