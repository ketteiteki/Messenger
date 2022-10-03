namespace Messenger.Domain.Entities;

public class ChatUser
{
	public Guid ChatId { get; set; }
	
	public Chat Chat { get; set; }
	
	public Guid UserId { get; set; }

	public User User { get; set; }

	public bool CanSendMedia { get; set; } = true;

	public DateTime? MuteDateOfExpire { get; set; }
	
	public RoleUserByChat? Role { get; set; }
}