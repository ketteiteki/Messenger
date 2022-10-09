using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiCommands.Messages;

public record UpdateMessageCommand(
	Guid RequestorId,
	Guid MessageId,
	string Text) 
	: IRequest<Result<MessageDto>>;