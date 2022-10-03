using MediatR;
using Messenger.BusinessLogic.Models;

namespace Messenger.BusinessLogic.Dialogs.Command;

public class DeleteDialogCommand : IRequest<ChatDto>
{
	public Guid RequesterId { get; set; }

	public Guid ChatId { get; set; }
	
	public bool IsForBoth { get; set; }
}