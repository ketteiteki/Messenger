using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiCommands.Channels;

public record DeleteChannelCommand(
		Guid RequesterId,
		Guid ChannelId)
	: IRequest<Result<ChatDto>>;