using MediatR;
using Messenger.BusinessLogic.Models;

namespace Messenger.BusinessLogic.Conversations.Commands;

public class UpdateConversationCommand : IRequest<ChatDto>
{
	public Guid RequesterId { get; set; }
	
	public Guid ChatId { get; set; }
	
	public string Name { get; set; }

	public string Title { get; set; }
}