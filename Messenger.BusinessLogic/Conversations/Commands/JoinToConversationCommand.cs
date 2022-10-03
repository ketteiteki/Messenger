using MediatR;
using Messenger.BusinessLogic.Models;

namespace Messenger.BusinessLogic.Conversations.Commands;

public class JoinToConversationCommand : IRequest<ChatDto>
{
	public Guid RequesterId { get; set; }
	
	public Guid ChatId { get; set; }
}