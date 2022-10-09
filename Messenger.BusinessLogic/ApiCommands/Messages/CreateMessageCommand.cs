using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Microsoft.AspNetCore.Http;

namespace Messenger.BusinessLogic.ApiCommands.Messages;

public record CreateMessageCommand(
	Guid RequestorId,
	string Text,
	Guid? ReplyToId,
	Guid ChatId,
	IFormFileCollection? Files
	) : IRequest<Result<MessageDto>>;