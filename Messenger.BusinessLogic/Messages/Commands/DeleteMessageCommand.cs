using MediatR;
using Messenger.BusinessLogic.Models;

namespace Messenger.BusinessLogic.Messages.Commands;

public class DeleteMessageCommand : IRequest<MessageDto>
{
	public Guid RequesterId { get; set; }

	public Guid MessageId { get; set; }
	
	public bool IsDeleteForAll { get; set; }
}