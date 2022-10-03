using Messenger.Domain.Entities;

namespace Messenger.BusinessLogic.Models;

public class AttachmentDto
{ 
	public Guid Id { get; init; }
    	
	public string Name { get; init; }
    	
	public long Size { get; init; }
    	
	public string Link { get; init; }

	public AttachmentDto(Attachment attachment)
	{
		Id = attachment.Id;
		Name = attachment.Name;
		Size = attachment.Size;
		Link = attachment.Link;
	} 
}