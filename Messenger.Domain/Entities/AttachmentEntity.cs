using FluentValidation;
using Messenger.Domain.Entities.Abstraction;
using Messenger.Domain.Entities.Validation;

namespace Messenger.Domain.Entities;

public class AttachmentEntity : IBaseEntity
{
	public Guid Id { get; set; } = Guid.NewGuid();
	
	public string FileName { get; set; }
	
	public long Size { get; set; }
	
	public Guid MessageId { get; set; }
	
	public MessageEntity Message { get; set; }
	
	public AttachmentEntity(string fileName, long size, Guid messageId)
	{
		FileName = fileName;
		Size = size;
		MessageId = messageId;
		
		new AttachmentEntityValidator().ValidateAndThrow(this);
	}
}