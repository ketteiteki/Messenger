namespace Messenger.Domain.Entities;

public class DeletedMessageByUser
{
	public Guid MessageId { get; set; }
	
	public Message Message { get; set; }

	public Guid UserId { get; set; }

	public User User { get; set; }
}