using MediatR;
using Messenger.BusinessLogic.Models;

namespace Messenger.BusinessLogic.Dialogs.Queries;

public class GetDialogQuery : IRequest<ChatDto>
{
	public Guid RequesterId { get; set; }

	public Guid WithWhomId { get; set; }
}