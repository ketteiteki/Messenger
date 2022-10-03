using MediatR;
using Messenger.BusinessLogic.Models;

namespace Messenger.BusinessLogic.Conversations.Commands;

public class AddUserToConversationCommand : IRequest<UserDto>
{
	public Guid RequestorId { get; set; }

	public Guid ChatId { get; set; }

	public Guid UserId { get; set; }
}