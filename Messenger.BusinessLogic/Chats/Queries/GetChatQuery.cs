using MediatR;
using Messenger.BusinessLogic.Models;

namespace Messenger.BusinessLogic.Chats.Queries;

public class GetChatQuery : IRequest<ChatDto>
{
	public Guid RequesterId { get; set; }
	
	public Guid ChatId { get; set; }
}