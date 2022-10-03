using MediatR;
using Messenger.BusinessLogic.Models;

namespace Messenger.BusinessLogic.Channels.Command;

public class DeleteChannelCommand : IRequest<ChatDto>
{
	public Guid RequesterId { get; set; }

	public Guid ChannelId { get; set; }
}