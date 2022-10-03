using MediatR;
using Messenger.BusinessLogic.Models;

namespace Messenger.BusinessLogic.Messages.Commands;

public class UpdateMessageCommand : IRequest<MessageDto>
{
	public Guid RequesterId { get; set; }
	
	public Guid MessageId { get; set; }
	
	public string Text { get; set; } = String.Empty;
}