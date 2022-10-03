using MediatR;
using Messenger.BusinessLogic.Models;

namespace Messenger.BusinessLogic.Dialogs.Command;

public class CreateDialogCommand : IRequest<ChatDto>
{
	public Guid RequesterId { get; set; }

	public Guid UserId { get; set; }
}