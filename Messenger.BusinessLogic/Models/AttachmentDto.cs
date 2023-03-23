namespace Messenger.BusinessLogic.Models;

public class AttachmentDto
{ 
	public Guid Id { get; init; }
    	
	public long Size { get; init; }
    	
	public string Link { get; init; }

	public AttachmentDto(Guid id, string link, long size)
	{
		Id = id;
		Link = link;
		Size = size;
	} 
}