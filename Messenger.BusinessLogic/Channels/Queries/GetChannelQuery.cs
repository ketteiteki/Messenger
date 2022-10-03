using MediatR;
using Messenger.BusinessLogic.Models;

namespace Messenger.BusinessLogic.Channels.Queries;

public record GetChannelQuery : IRequest<ChatDto> 
{
	public Guid RequesterId { get; set; }
	
	public Guid ChannelId { get; set; }
}