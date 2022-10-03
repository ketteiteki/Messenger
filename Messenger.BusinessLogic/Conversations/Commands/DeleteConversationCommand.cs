using MediatR;
using Messenger.BusinessLogic.Models;

namespace Messenger.BusinessLogic.Conversations.Commands;

public class DeleteConversationCommand : IRequest<ChatDto>
{
	public Guid ChatId { get; set; }

	public Guid RequesterId { get; set; }
}