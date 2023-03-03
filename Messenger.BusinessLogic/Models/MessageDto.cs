
namespace Messenger.BusinessLogic.Models;

public class MessageDto
{
	public Guid Id { get; init; }

	public string Text { get; init; }
	
	public bool IsEdit { get; init; }
	
	public Guid? OwnerId { get; init; }

	public string OwnerDisplayName { get; init; }
	
	public string OwnerAvatarLink { get; init; }

	public Guid? ReplyToMessageId { get; init; }
	
	public string ReplyToMessageText { get; init; }
	
	public string ReplyToMessageAuthorDisplayName { get; init; }

	public List<AttachmentDto> Attachments { get; init; } = new();
	
	public Guid ChatId { get; init; }

	public DateTime DateOfCreate { get; init; }
}