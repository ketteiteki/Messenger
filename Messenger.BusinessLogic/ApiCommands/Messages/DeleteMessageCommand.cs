using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiCommands.Messages;

public record DeleteMessageCommand(
	Guid RequestorId,
	Guid MessageId,
	bool IsDeleteForAll) 
	: IRequest<Result<MessageDto>>;