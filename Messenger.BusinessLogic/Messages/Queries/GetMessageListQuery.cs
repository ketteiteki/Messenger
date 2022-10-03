using MediatR;
using Messenger.BusinessLogic.Models;

namespace Messenger.BusinessLogic.Messages.Queries;

public class GetMessageListQuery : IRequest<List<MessageDto>>
{
	public Guid RequesterId { get; set; }

	public Guid ChatId { get; set; }
	
	public int Limit { get; set; }
	
	public DateTime? FromMessageDateTime { get; set; }
}
	