using MediatR;
using Messenger.BusinessLogic.Models;
using Messenger.BusinessLogic.Responses;

namespace Messenger.BusinessLogic.ApiQueries.Channels;

public record GetChannelQuery(
	Guid RequesterId,
	Guid ChannelId)
	: IRequest<Result<ChatDto>>;