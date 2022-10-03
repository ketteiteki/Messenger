using MediatR;
using Messenger.BusinessLogic.Models;
using Microsoft.AspNetCore.Http;

namespace Messenger.BusinessLogic.Messages.Commands;

public class CreateMessageCommand : IRequest<MessageDto>
{
	public Guid RequestorId { get; set; }
	
	public string Text { get; set; } = String.Empty;
	
	public Guid? ReplyToId { get; set; }
	
	public Guid ChatId { get; set; }
	
	public IFormFileCollection? Files { get; set; }
}