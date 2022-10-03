using MediatR;
using Messenger.BusinessLogic.Models;

namespace Messenger.BusinessLogic.Messages.Queries;

public class GetMessageListBySearchQuery : IRequest<List<MessageDto>>
{
	public Guid RequesterId { get; set; }
	
	public Guid ChatId { get; set; }
	
	public int Limit { get; set; }
	
	public DateTime? FromMessageDateOfCreate { get; set; }
	
	public Guid? FromUserId { get; set; }
	
	public string SearchText { get; set; }
}
