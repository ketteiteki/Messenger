using FluentValidation;
using Messenger.Domain.Entities.Abstraction;
using Messenger.Domain.Entities.Validation;

namespace Messenger.Domain.Entities;

public class AttachmentEntity : IBaseEntity
{
	public Guid Id { get; set; }
	
	public string Name { get; set; }
	
	public long Size { get; set; }
	
	public string Link { get; set; }
	
	public Guid MessageId { get; set; }
	
	public MessageEntity Message { get; set; }
	
	public AttachmentEntity(string name, long size, string link, Guid messageId)
	{
		Name = name;
		Size = size;
		Link = link;
		MessageId = messageId;
		
		new AttachmentEntityValidator().ValidateAndThrow(this);
	}
}