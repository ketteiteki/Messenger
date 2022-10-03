using FluentValidation;
using Messenger.Domain.Entities.Abstraction;
using Messenger.Domain.Entities.Validation;

namespace Messenger.Domain.Entities;

public class Attachment : IBaseEntity
{
	public Guid Id { get; set; }
	
	public string Name { get; set; }
	
	public long Size { get; set; }
	
	public string Link { get; set; }
	
	public Guid MessageId { get; set; }
	
	public Message Message { get; set; }
	
	public Attachment(string name, long size, string link, Guid messageId)
	{
		Name = name;
		Size = size;
		Link = link;
		MessageId = messageId;
		
		new AttachmentValidator().ValidateAndThrow(this);
	}
}