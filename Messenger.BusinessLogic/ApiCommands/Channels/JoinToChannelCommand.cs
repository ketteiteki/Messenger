using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiCommands.Channels;

public record JoinToChannelCommand(
	Guid RequestorId,
	Guid ChannelId) 
	: IRequest<Result<ChatDto>>;