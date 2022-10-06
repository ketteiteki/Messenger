using MediatR;
using Messenger.BusinessLogic.Models;

namespace Messenger.BusinessLogic.Conversations.Commands;

public class CreatePermissionsUserInConversationCommand : IRequest<PermissionDto>
{
	public Guid RequesterId { get; set; }
	
	public Guid ChatId { get; set; }

	public Guid UserId { get; set; }
	
	public bool CanSendMedia { get; set; }
}