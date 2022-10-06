using MediatR;
using Messenger.BusinessLogic.Models;

namespace Messenger.BusinessLogic.Conversations.Queries;

public class GetConversationQuery : IRequest<ChatDto>
{
	public Guid RequesterId { get; set; }

	public Guid ChatId { get; set; }
}