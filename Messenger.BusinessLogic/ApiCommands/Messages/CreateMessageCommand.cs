using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;
using Microsoft.AspNetCore.Http;

namespace Messenger.BusinessLogic.ApiCommands.Messages;

public record CreateMessageCommand(
	Guid RequesterId,
	string Text,
	Guid? ReplyToId,
	Guid ChatId,
	List<IFormFile> Files
	) : IRequest<Result<MessageDto>>;