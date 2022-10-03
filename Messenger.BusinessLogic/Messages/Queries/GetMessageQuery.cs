using MediatR;
using Messenger.BusinessLogic.Models;

namespace Messenger.BusinessLogic.Messages.Queries;

public class GetMessageQuery : IRequest<MessageDto>
{
	public Guid RequesterId { get; set; }
	
	public Guid ChatId { get; set; }
	
	public Guid MessageId { get; set; }
}