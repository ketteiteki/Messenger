using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiCommands.Chats;

public record UpdateChatDataCommand(
	Guid RequesterId,
	Guid ChatId,
	string Name,
	string Title
	) : IRequest<Result<ChatDto>>;